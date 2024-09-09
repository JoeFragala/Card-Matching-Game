using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MemoryMatchingGame
{
    public partial class MainWindow : Window
    {
        private Dictionary<string, List<string>> GameThemes;
        private List<Button> flippedCards = new List<Button>(); // Track currently flipped cards
        private bool isChecking = false; // Flag to prevent multiple reveals during checking
        private string currentTheme; // Track the current theme
        private string currentDifficulty; // Track the current difficulty

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize GameThemes dictionary
            GameThemes = new Dictionary<string, List<string>>
            {
                { "Colors", new List<string> { "Red", "Orange", "Yellow", "Green", "Blue", "Indigo", "Violet", "White", "Grey", "Black", "Brown", "Pink", "Sky Blue", "Navy", "Tan" } },
                { "Food", new List<string> { "Apple", "Orange", "Lemon", "Watermelon", "Blueberry", "Grapes", "Plum", "Banana", "Raspberry", "Blackberry", "Kiwi", "Peach", "Mango", "Nectarine", "Honeydew" } },
                { "Animals", new List<string> { "Frog", "Tiger", "Cat", "Dog", "Hamster", "Fish", "Whale", "Monkey", "Elephant", "Giraffe", "Deer", "Bear", "Bird", "Dolphin", "Lion" } }
            };
        }

        // Event handler for start button click
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear the current UI
            MainGrid.Children.Clear();

            // Create a new StackPanel for theme selection
            StackPanel themePanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Add a text block for the difficulty selection
            TextBlock themeText = new TextBlock
            {
                Text = "Select Theme",
                FontSize = 24,
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            themePanel.Children.Add(themeText);

            // Add theme buttons dynamically
            foreach (var theme in GameThemes.Keys)
            {
                Button themeButton = new Button
                {
                    Content = theme,
                    Width = 250,
                    Height = 400,
                    Margin = new Thickness(10)
                };

                // Attach click event for each theme button
                themeButton.Click += ThemeButton_Click;
                themePanel.Children.Add(themeButton);
            }

            // Add the theme panel to the main grid
            MainGrid.Children.Add(themePanel);
        }

        // Event handler for theme button click
        private void ThemeButton_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected theme from the button content
            Button clickedButton = sender as Button;
            string selectedTheme = clickedButton.Content.ToString();

            // Show the difficulty selection screen
            ShowDifficultySelection(selectedTheme);
        }

        // Method to show difficulty selection based on the selected theme
        private void ShowDifficultySelection(string theme)
        {
            // Store the selected theme
            currentTheme = theme;

            // Clear the current UI
            MainGrid.Children.Clear();

            // Create a new StackPanel for difficulty selection
            StackPanel difficultyPanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Add a text block for the difficulty selection
            TextBlock difficultyText = new TextBlock
            {
                Text = "Select Difficulty",
                FontSize = 24,
                Margin = new Thickness(0, 0, 0, 20),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            difficultyPanel.Children.Add(difficultyText);

            // Add difficulty buttons dynamically
            string[] difficulties = { "3x4", "4x5", "5x6" };
            foreach (var difficulty in difficulties)
            {
                Button difficultyButton = new Button
                {
                    Content = difficulty,
                    Width = 250,
                    Height = 400,
                    Margin = new Thickness(10)
                };

                // Attach click event for each difficulty button
                difficultyButton.Click += (s, e) => StartGame(currentTheme, difficulty);
                difficultyPanel.Children.Add(difficultyButton);
            }

            // Add the Back button
            Button backButton = new Button
            {
                Content = "Back",
                Width = 250,
                Height = 400,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Attach click event to navigate back to theme selection
            backButton.Click += BackButton_Click;

            difficultyPanel.Children.Add(backButton);

            // Add the difficulty panel to the main grid
            MainGrid.Children.Add(difficultyPanel);
        }

        // Event handler for the Back button click
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Go back to theme selection
            StartButton_Click(sender, e);
        }

        // Method to start the game based on selected theme and difficulty
        private void StartGame(string theme, string difficulty)
        {
            // Store the current theme and difficulty
            currentTheme = theme;
            currentDifficulty = difficulty;

            // Clear the current UI
            MainGrid.Children.Clear();

            // Parse difficulty to set rows and columns
            (int rows, int columns) = ParseDifficulty(difficulty);
            int pairs = rows * columns / 2;

            // Get theme contents and create pairs of cards
            var themeContents = GameThemes[theme];
            var cardContents = GenerateCardContents(themeContents, pairs);

            // Shuffle cards
            Shuffle(cardContents);

            // Set up the game board
            SetUpGameBoard(rows, columns, cardContents);
        }

        // Parse difficulty string to rows and columns
        private (int, int) ParseDifficulty(string difficulty)
        {
            string[] parts = difficulty.Split('x');
            int rows = int.Parse(parts[0]);
            int columns = int.Parse(parts[1]);
            return (rows, columns);
        }

        // Generate a list of card contents with pairs based on the number of pairs needed
        private List<string> GenerateCardContents(List<string> themeContents, int pairCount)
        {
            var selectedContents = new List<string>(themeContents);
            var cardContents = new List<string>();

            for (int i = 0; i < pairCount; i++)
            {
                cardContents.Add(selectedContents[i]);
                cardContents.Add(selectedContents[i]); // Add pair
            }

            return cardContents;
        }

        // Shuffle the card contents
        private void Shuffle(List<string> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                string value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        // Set up the game board with the specified rows, columns, and card contents
        private void SetUpGameBoard(int rows, int columns, List<string> cardContents)
        {
            // Create a new Grid
            Grid gameGrid = new Grid();

            // Set up rows and columns
            for (int i = 0; i < rows; i++)
                gameGrid.RowDefinitions.Add(new RowDefinition());

            for (int j = 0; j < columns; j++)
                gameGrid.ColumnDefinitions.Add(new ColumnDefinition());

            // Add cards to the grid
            int index = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Button cardButton = new Button
                    {
                        Content = "?", // Initially show a question mark
                        FontSize = 24,
                        Tag = cardContents[index], // Store the actual card content in the Tag
                        Margin = new Thickness(5)
                    };
                    cardButton.Click += CardButton_Click; // Attach click event

                    Grid.SetRow(cardButton, i);
                    Grid.SetColumn(cardButton, j);
                    gameGrid.Children.Add(cardButton);
                    index++;
                }
            }

            // Add the game grid to the main UI
            MainGrid.Children.Add(gameGrid);
        }

        // Event handler for card button click
        private void CardButton_Click(object sender, RoutedEventArgs e)
        {
            if (isChecking) return; // Prevent action if currently checking cards

            Button clickedButton = sender as Button;

            // Check if the card is already flipped
            if (flippedCards.Contains(clickedButton) || clickedButton.Content.ToString() != "?")
                return;

            // Reveal the card content
            clickedButton.Content = clickedButton.Tag.ToString();

            // Add to flipped cards
            flippedCards.Add(clickedButton);

            // Check if two cards are flipped
            if (flippedCards.Count == 2)
            {
                isChecking = true; // Set flag to prevent further clicks

                // Check for match
                if (flippedCards[0].Tag.ToString() == flippedCards[1].Tag.ToString())
                {
                    // Match found, leave cards revealed
                    flippedCards.Clear();
                    isChecking = false; // Reset flag
                    CheckForWin();
                }
                else
                {
                    // No match, hide cards again after a short delay
                    Dispatcher.InvokeAsync(async () =>
                    {
                        await Task.Delay(1000);
                        foreach (var card in flippedCards)
                        {
                            card.Content = "?"; // Hide the card again
                        }
                        flippedCards.Clear();
                        isChecking = false; // Reset flag
                    });
                }
            }
        }

        // Check if all pairs are matched
        private void CheckForWin()
        {
            // Check if all cards are revealed (no card showing "?")
            bool allMatched = true;
            foreach (var element in MainGrid.Children)
            {
                if (element is Grid grid) // Check inside the grid for buttons
                {
                    foreach (var item in grid.Children)
                    {
                        if (item is Button button && button.Content.ToString() == "?")
                        {
                            allMatched = false;
                            break;
                        }
                    }
                }
            }

            if (allMatched)
            {
                MessageBox.Show("Congratulations! You've matched all pairs!", "You Win!", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowRestartOptions();
            }
        }

        // Show options to restart or go back to the main menu
        private void ShowRestartOptions()
        {
            MainGrid.Children.Clear();

            StackPanel endGamePanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Restart Button
            Button restartButton = new Button
            {
                Content = "Play Again!",
                Width = 250,
                Height = 400,
                Margin = new Thickness(10)
            };
            restartButton.Click += (sender, e) => StartGame(currentTheme, currentDifficulty); // Restart the game with the same settings
            endGamePanel.Children.Add(restartButton);

            // Change Difficulty Button
            Button changeDifficultyButton = new Button
            {
                Content = "Change Difficulty",
                Width = 250,
                Height = 400,
                Margin = new Thickness(10)
            };
            changeDifficultyButton.Click += (sender, e) => ShowDifficultySelection(currentTheme); // Show difficulty selection with the current theme
            endGamePanel.Children.Add(changeDifficultyButton);

            // Main Menu Button
            Button mainMenuButton = new Button
            {
                Content = "Change Theme",
                Width = 250,
                Height = 400,
                Margin = new Thickness(10)
            };
            mainMenuButton.Click += (sender, e) => ShowThemeSelection(); // Return to theme selection
            endGamePanel.Children.Add(mainMenuButton);

            MainGrid.Children.Add(endGamePanel);
        }

        // Method to show the theme selection screen
        private void ShowThemeSelection()
        {
            // Clear the current UI
            MainGrid.Children.Clear();

            // Create a new StackPanel for theme selection
            StackPanel themePanel = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            // Add theme buttons dynamically
            foreach (var theme in GameThemes.Keys)
            {
                Button themeButton = new Button
                {
                    Content = theme,
                    Width = 250,
                    Height = 400,
                    Margin = new Thickness(10)
                };

                // Attach click event for each theme button
                themeButton.Click += ThemeButton_Click;
                themePanel.Children.Add(themeButton);
            }

            // Add the theme panel to the main grid
            MainGrid.Children.Add(themePanel);
        }
    }
}
