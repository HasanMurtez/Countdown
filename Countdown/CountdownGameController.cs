using System;
using System.Collections.Generic;
using System.Linq;

namespace Countdown
{
    public class CountdownGameController
    {
        private List<char> _vowels;
        private List<char> _consonants;
        private List<char> _selectedLetters;
        private Dictionary<string, int> _playerScores;
        private HashSet<string> _dictionary;

        private Label _player1ScoreLabel;
        private Label _player2ScoreLabel;
        private Label _roundWinnerLabel;
        private Label _gameResultLabel;

        public string Player1Name { get; private set; }
        public string Player2Name { get; private set; }

        public int SelectedLettersCount => _selectedLetters.Count;

        private Dictionary<string, int> _currentRoundScores;

        public CountdownGameController(
            string player1Name,
            string player2Name,
            Label player1ScoreLabel,
            Label player2ScoreLabel,
            Label roundWinnerLabel,
            Label gameResultLabel)
            {
            Player1Name = player1Name;
            Player2Name = player2Name;

            _playerScores = new Dictionary<string, int>
            {
                { player1Name, 0 },
                { player2Name, 0 }
            };

                    _currentRoundScores = new Dictionary<string, int>
            {
                { player1Name, 0 },
                { player2Name, 0 }
            };

            _selectedLetters = new List<char>();
            _dictionary = LoadDictionary(); 

            InitializeLetters();

            _player1ScoreLabel = player1ScoreLabel;
            _player2ScoreLabel = player2ScoreLabel;
            _roundWinnerLabel = roundWinnerLabel;
            _gameResultLabel = gameResultLabel;
        }

        private void ResetRoundScores()
        {
            _currentRoundScores[Player1Name] = 0;
            _currentRoundScores[Player2Name] = 0;
        }


        public void UpdateScoreLabels()
        {
            _player1ScoreLabel.Text = $"{Player1Name}: {_playerScores[Player1Name]} points";
            _player2ScoreLabel.Text = $"{Player2Name}: {_playerScores[Player2Name]} points";

            _player1ScoreLabel.IsVisible = true;
            _player2ScoreLabel.IsVisible = true;
        }

        public void UpdateRoundWinnerLabel(string message)
        {
            _roundWinnerLabel.Text = message;
            _roundWinnerLabel.IsVisible = true;
        }

        public void UpdateGameResultLabel(string message)
        {
            _gameResultLabel.Text = message;
            _gameResultLabel.IsVisible = true;
        }

        // Other properties and methods...

        public int GetPlayerScore(string playerName)
        {
            return _playerScores.ContainsKey(playerName) ? _playerScores[playerName] : 0;
        }
        private void InitializeLetters()
        {
            _vowels = new List<char> { 'A', 'E', 'I', 'O', 'U' };
            _consonants = "BCDFGHJKLMNPQRSTVWXYZ".ToCharArray().ToList();
        }

        public void PickLetter(string letterType)
        {
            if (_selectedLetters.Count >= 9)
                throw new InvalidOperationException("Cannot pick more than 9 letters.");

            Random random = new Random();
            char letter = letterType == "Consonant" ?
                          _consonants[random.Next(_consonants.Count)] :
                          _vowels[random.Next(_vowels.Count)];

            _selectedLetters.Add(letter);
        }

        public List<char> GetSelectedLetters()
        {
            return _selectedLetters;
        }

        private int CalculateScore(string word)
        {
            // Dictionary with point values assigned based on letter frequency
            Dictionary<char, int> letterScores = new Dictionary<char, int>
            {
                // Vowels
                {'A', 15}, {'E', 21}, {'I', 13}, {'O', 13}, {'U', 5},
                // Consonants
                {'B', 2}, {'C', 3}, {'D', 6}, {'F', 2}, {'G', 3},
                {'H', 2}, {'J', 1}, {'K', 1}, {'L', 5}, {'M', 4},
                {'N', 8}, {'P', 4}, {'Q', 1}, {'R', 9}, {'S', 9},
                {'T', 9}, {'V', 1}, {'W', 1}, {'X', 1}, {'Y', 1},
                {'Z', 1}
            };

            int score = 0;
            foreach (char c in word.ToUpper())
            {
                if (letterScores.ContainsKey(c))
                {
                    score += letterScores[c];
                }
            }

            return score;
        }



        public void SubmitWord(string playerName, string word)
        {
            if (IsValidWord(word))
            {
                int score = CalculateScore(word);
                _currentRoundScores[playerName] = score; // Set round score
            }
        }


        private bool IsValidWord(string word)
        {
            var wordChars = word.ToUpper().ToCharArray();
            var tempLetters = new List<char>(_selectedLetters);

            foreach (var letter in wordChars)
            {
                if (!tempLetters.Remove(letter))
                {
                    return false;
                }
            }

            return _dictionary.Contains(word.ToUpper());
        }

        public void CalculateRoundWinner()
        {
            // Update total scores with round scores
            _playerScores[Player1Name] += _currentRoundScores[Player1Name];
            _playerScores[Player2Name] += _currentRoundScores[Player2Name];

            // Determine the round winner
            string roundWinnerMessage;
            if (_currentRoundScores[Player1Name] > _currentRoundScores[Player2Name])
            {
                roundWinnerMessage = $"{Player1Name} wins this round!";
            }
            else if (_currentRoundScores[Player2Name] > _currentRoundScores[Player1Name])
            {
                roundWinnerMessage = $"{Player2Name} wins this round!";
            }
            else
            {
                roundWinnerMessage = "This round is a tie!";
            }

            // Reset the round scores for the next round
            ResetRoundScores();

            // Update the UI
            UpdateScoreLabels();
            UpdateRoundWinnerLabel(roundWinnerMessage);
        }


        // Method to update the score labels






        public string GetWinner()
        {
            int player1Score = _playerScores[Player1Name];
            int player2Score = _playerScores[Player2Name];

            // Check and print final scores for debugging (Optional)
            Console.WriteLine($"Final Scores:");
            Console.WriteLine($"{Player1Name}: {player1Score} points");
            Console.WriteLine($"{Player2Name}: {player2Score} points");

            // Determine the final winner
            string finalWinnerMessage;
            if (player1Score > player2Score)
                finalWinnerMessage = $"{Player1Name} wins the entire game!";
            else if (player2Score > player1Score)
                finalWinnerMessage = $"{Player2Name} wins the entire game!";
            else
                finalWinnerMessage = "The game is a tie!";

            // Update the UI
            UpdateScoreLabels();
            UpdateGameResultLabel(finalWinnerMessage);

            return finalWinnerMessage;
        }

        



        public void ResetLetters()
        {
            _selectedLetters.Clear();
        }

        private HashSet<string> LoadDictionary()
        {
            var dictionary = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Get the base directory of the application
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Combine base directory with relative path to dictionary file
            string filePath = Path.Combine(baseDirectory, "dictionary.txt");

            // Debugging: Output the full file path
            Console.WriteLine($"Looking for dictionary file at: {filePath}");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Dictionary file not found.", filePath);
            }

            foreach (var line in File.ReadLines(filePath))
            {
                dictionary.Add(line.Trim());
            }

            return dictionary;
        }



    }
}
