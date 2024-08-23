using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Countdown
{
    public partial class MainPage : ContentPage
    {
        private CountdownGameController gameController;
        private string currentPlayer;
        private int currentRound = 0;
        private const int totalRounds = 6;
        private int timerDuration = 30; // 30 seconds
        private string historyFilePath;


        public MainPage()
        {
            InitializeComponent();
            historyFilePath = Path.Combine(FileSystem.AppDataDirectory, "game_history.txt");

        }

        private void OnStartGameClicked(object sender, EventArgs e)
        {
            string player1Name = Player1NameEntry.Text;
            string player2Name = Player2NameEntry.Text;

            // Pass the UI elements (labels) to the controller
            gameController = new CountdownGameController(
                player1Name,
                player2Name,
                Player1ScoreLabel,
                Player2ScoreLabel,
                RoundWinnerLabel,
                GameResultLabel
            );

            SelectFirstPlayer();

            StartGameButton.IsVisible = false;
            Player1NameEntry.IsVisible = false;
            Player2NameEntry.IsVisible = false;

            TurnLabel.IsVisible = true;
            SelectedLettersLabel.IsVisible = true;
            PickConsonantButton.IsVisible = true;
            PickVowelButton.IsVisible = true;
            TurnLabel.Text = $"{currentPlayer}'s turn to pick letters.";
        }

        private void SelectFirstPlayer()
        {
            Random rand = new Random();
            currentPlayer = rand.Next(0, 2) == 0 ? gameController.Player1Name : gameController.Player2Name;
        }

        private void OnPickConsonantClicked(object sender, EventArgs e)
        {
            PickLetter("Consonant");
        }

        private void OnPickVowelClicked(object sender, EventArgs e)
        {
            PickLetter("Vowel");
        }

        private void PickLetter(string letterType)
        {
            gameController.PickLetter(letterType);
            SelectedLettersLabel.Text = string.Join(" ", gameController.GetSelectedLetters());

            if (gameController.SelectedLettersCount == 9)
            {
                PickConsonantButton.IsVisible = false;
                PickVowelButton.IsVisible = false;

                StartTimer();
            }
        }

        private async void StartTimer()
        {
            timerDuration = 30; // Reset timer for each round
            TimerLabel.IsVisible = true;

            PlayCountdownSound();  // Start playing the countdown sound

            while (timerDuration > 0)
            {
                TimerLabel.Text = $"Timer: {timerDuration--}";
                await Task.Delay(1000);
            }

            TimerLabel.IsVisible = false;
            StopCountdownSound();  // Stop the sound when the timer ends

            Player1WordEntry.IsVisible = true;
            SubmitPlayer1WordButton.IsVisible = true;
            Player2WordEntry.IsVisible = true;
            SubmitPlayer2WordButton.IsVisible = true;
        }

        private void PlayCountdownSound()
        {
            
            CountdownMediaElement.Play();


        }

        private void StopCountdownSound()
        {
            CountdownMediaElement.Stop();
        }



        private void OnSubmitPlayer1WordClicked(object sender, EventArgs e)
        {
            string word = Player1WordEntry.Text;
            gameController.SubmitWord(gameController.Player1Name, word);
            Player1WordEntry.IsVisible = false;
            SubmitPlayer1WordButton.IsVisible = false;
            gameController.UpdateScoreLabels();  // Add this line to update the score labels after submission
            CheckForNextRound();
        }

        private void OnSubmitPlayer2WordClicked(object sender, EventArgs e)
        {
            string word = Player2WordEntry.Text;
            gameController.SubmitWord(gameController.Player2Name, word);
            Player2WordEntry.IsVisible = false;
            SubmitPlayer2WordButton.IsVisible = false;
            gameController.UpdateScoreLabels(); ;  // Add this line to update the score labels after submission
            CheckForNextRound();
        }




        private void CheckForNextRound()
        {
            if (!Player1WordEntry.IsVisible && !Player2WordEntry.IsVisible)
            {
                gameController.CalculateRoundWinner();
                gameController.UpdateScoreLabels();

                currentRound++;

                if (currentRound >= totalRounds)
                {
                    EndGame();
                }
                else
                {
                    NextRoundButton.IsVisible = true;
                }
            }
        }


        private void OnNextRoundClicked(object sender, EventArgs e)
        {
            NextRoundButton.IsVisible = false;
            SelectFirstPlayer();
            TurnLabel.Text = $"{currentPlayer}'s turn to pick letters.";

            gameController.ResetLetters();
            SelectedLettersLabel.Text = "";

            PickConsonantButton.IsVisible = true;
            PickVowelButton.IsVisible = true;

            // Call the CalculateRoundWinner method
            gameController.CalculateRoundWinner();
            
            
        }

        private void EndGame()
        {
            TurnLabel.IsVisible = false;
            SelectedLettersLabel.IsVisible = false;
            WinnerLabel.Text = gameController.GetWinner();
            WinnerLabel.IsVisible = true;
            RestartGameButton.IsVisible = true;
            gameController.UpdateScoreLabels();

            // Record game history
            var historyEntry = new GameHistory
            {
                Timestamp = DateTime.Now,
                Player1Name = gameController.Player1Name,
                Player1Score = gameController.GetPlayerScore(gameController.Player1Name),
                Player2Name = gameController.Player2Name,
                Player2Score = gameController.GetPlayerScore(gameController.Player2Name)
            };

            AppendHistoryToFile(historyEntry);
        }

        private void AppendHistoryToFile(GameHistory history)
        {
            var historyEntry = $"{history.Timestamp:G} - {history.Player1Name}: {history.Player1Score} vs {history.Player2Name}: {history.Player2Score}{Environment.NewLine}";
            File.AppendAllText(historyFilePath, historyEntry, Encoding.UTF8);
        }

        private async void OnViewHistoryClicked(object sender, EventArgs e)
        {
            // Read the history file content
            if (File.Exists(historyFilePath))
            {
                var historyContent = File.ReadAllText(historyFilePath);
                await DisplayAlert("Game History", historyContent, "OK");
            }
            else
            {
                await DisplayAlert("Game History", "No history available.", "OK");
            }
        }

        private void OnRestartGameClicked(object sender, EventArgs e)
        {
            currentRound = 0;
            WinnerLabel.IsVisible = false;
            RestartGameButton.IsVisible = false;
            StartGameButton.IsVisible = true;
            Player1NameEntry.IsVisible = true;
            Player2NameEntry.IsVisible = true;
            TurnLabel.Text = "";
            // Call the GetWinner method
            string result = gameController.GetWinner();

            // Update WinnerLabel based on the result
            WinnerLabel.Text = result;
            WinnerLabel.IsVisible = true;

            // Show RestartGameButton if needed
            RestartGameButton.IsVisible = true;
        }

        

    }

    public class GameHistory
    {
        public DateTime Timestamp { get; set; }
        public string Player1Name { get; set; }
        public int Player1Score { get; set; }
        public string Player2Name { get; set; }
        public int Player2Score { get; set; }
    }
}
