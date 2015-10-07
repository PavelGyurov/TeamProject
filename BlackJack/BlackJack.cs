using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace BlackJack
{
    class BlackJack
    {
        const char spade = '\u2660';
        const char heart = '\u2665';
        const char diamond = '\u2666';
        const char club = '\u2663';
        const string saveGamePath = "../../saveGame.txt";

        static void Main(string[] args)
        {
            Console.SetWindowSize(120, 50);
            Console.BufferHeight = Console.WindowHeight;
            Console.BufferWidth = Console.WindowWidth;

            if (!File.Exists(saveGamePath))
            {
                FileStream createSaveGame = new FileStream(saveGamePath, FileMode.Create, FileAccess.ReadWrite);
                createSaveGame.Close();
            }
            StreamReader readSaveGame = new StreamReader(saveGamePath);
            int chips = 1000;
            int winCount = 0, loseCount = 0, tieCount = 0;
            using (readSaveGame)
            {
                string line = readSaveGame.ReadLine();
                if (line != null)
                {
                    chips = int.Parse(line);
                    string wins = readSaveGame.ReadLine();
                    winCount = int.Parse(wins);
                    string losses = readSaveGame.ReadLine();
                    loseCount = int.Parse(losses);
                    string ties = readSaveGame.ReadLine();
                    tieCount = int.Parse(ties);
                }

            }
            if (chips == 0)
            {
                chips = NewGame();
                if (chips == 0) return;
            }
            List<string> deck = InitialiseDecks();

            while (chips > 0)
            {
                List<string> dealerCards = new List<string>();
                List<string> playerCards = new List<string>();
                int bet;
                Console.Clear();

                DrawTable(chips, 0);
                Console.WriteLine("You had {0} wins, {1} ties and {2} losses by now", winCount, tieCount, loseCount);
                while (true)
                {

                    Console.WriteLine("Enter the amount of chips you want to bet (you have {0} chips)", chips);
                    try
                    {
                        bet = int.Parse(Console.ReadLine());
                        if (bet < 0 || bet > chips)
                        {
                            Console.Clear();
                            DrawTable(chips, 0);
                            Console.WriteLine("You can't bet {0}, you have {1}", bet, chips);
                        }
                        if (bet > 0 && bet <= chips) break;
                    }
                    catch (System.FormatException)
                    {
                        Console.Clear();
                        DrawTable(chips, 0);
                        Console.WriteLine("Invalid input, please use a number.");
                    }
                }
                bool win = true;
                bool gameover = false;
                Console.Clear();
                dealerCards.Add(DrawCard(deck));
                playerCards.Add(DrawCard(deck));
                playerCards.Add(DrawCard(deck));

                PrintScore(playerCards, dealerCards); DrawTable(chips, bet);

                while (true)
                {
                    Console.WriteLine("Press H to Hit or S to Stand");
                    ConsoleKeyInfo choice = Console.ReadKey(true);
                    while (choice.Key != ConsoleKey.H && choice.Key != ConsoleKey.S)
                    {
                        Console.Clear();
                        PrintScore(playerCards, dealerCards); DrawTable(chips, bet);
                        Console.WriteLine("Press H to Hit or S to Stand");
                        choice = Console.ReadKey(true);
                    }
                    if (choice.Key == ConsoleKey.H)
                    {

                        playerCards.Add(DrawCard(deck));

                        Console.Clear();
                        PrintScore(playerCards, dealerCards); DrawTable(chips, bet);
                    }
                    if (GetScore(playerCards) == 21)
                    {
                        Console.WriteLine("Blackjack!");
                        win = true;
                        gameover = true;
                        winCount++;
                        Thread.Sleep(2000);
                        break;
                    }

                    if (GetScore(playerCards) > 21)
                    {
                        Console.WriteLine("Player busts");
                        win = false;
                        gameover = true;
                        loseCount++;

                        break;
                    }
                    if (choice.Key == ConsoleKey.S)
                    {

                        while (true)
                        {
                            if (GetScore(dealerCards) <= 16)
                            {
                                Console.WriteLine("Dealer hits");
                                dealerCards.Add(DrawCard(deck));
                                Thread.Sleep(700);

                                Console.Clear(); PrintScore(playerCards, dealerCards); DrawTable(chips, bet);
                            }
                            else if (GetScore(dealerCards) >= 17 && GetScore(dealerCards) <= 20)
                            {
                                Console.WriteLine("Dealer stands");
                                Thread.Sleep(700);

                                Console.Clear(); PrintScore(playerCards, dealerCards); DrawTable(chips, bet);
                                break;
                            }
                            if (GetScore(dealerCards) == 21)
                            {
                                Console.WriteLine("Dealer Blackjack!");
                                win = false;
                                gameover = true;
                                loseCount++;
                                break;
                            }

                            if (GetScore(dealerCards) > 21)
                            {
                                Console.WriteLine("Dealer busts");
                                win = true;
                                gameover = true;
                                winCount++;
                                break;
                            }
                            Thread.Sleep(1000);
                        }
                        break;
                    }
                }
                bool tie = false;
                if (GetScore(playerCards) == GetScore(dealerCards))
                {
                    Console.WriteLine("Tie");
                    gameover = true;
                    tie = true;
                    tieCount++;
                    Thread.Sleep(2000);
                }

                if (gameover == false)
                {
                    if (GetScore(playerCards) > GetScore(dealerCards))
                    {
                        win = true;
                        winCount++;
                    }
                    else if (GetScore(playerCards) < GetScore(dealerCards))
                    {
                        win = false;
                        loseCount++;
                    }
                }
                if (tie == false)
                {
                    if (win == true)
                    {
                        if (GetScore(playerCards) == 21)
                        {
                            Console.WriteLine("Player wins");
                            chips = Convert.ToInt32(chips + bet * 2.5);
                        }
                        else
                        {
                            Console.WriteLine("Player wins");
                            chips += bet;
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Dealer wins");
                        chips -= bet;
                        Thread.Sleep(2000);

                    }
                }
                if (chips == 0)
                {
                    chips = NewGame();
                }



                StreamWriter writeSaveGame = new StreamWriter(saveGamePath);
                using (writeSaveGame)
                {
                    writeSaveGame.WriteLine(chips);
                    writeSaveGame.WriteLine(winCount);
                    writeSaveGame.WriteLine(loseCount);
                    writeSaveGame.WriteLine(tieCount);
                }
            }
        }

        static int NewGame()
        {
            Console.WriteLine("You have 0 chips, do you want to start over ? (yes/no)");
            int chips;
            while (true)
            {
                string input = Console.ReadLine().ToLower();
                if (input == "yes")
                {
                    chips = 1000;
                    Console.Clear();
                    break;
                }
                if (input == "no")
                {
                    Console.WriteLine("Good bye! Thank you for playing !");
                    return 0;
                }
                if (input != "yes" || input != "no")
                {
                    Console.WriteLine("Invalid input, please type yes or no.");
                }
            }
            return chips;
        }

        static List<string> InitialiseDecks()
        {
            List<string> deck = new List<string>();
            for (int j = 0; j < 2; j++)
            {
                for (int i = 2; i < 11; i++)
                {
                    deck.Add(i.ToString() + spade);
                    deck.Add(i.ToString() + heart);
                    deck.Add(i.ToString() + diamond);
                    deck.Add(i.ToString() + club);
                }
                string[] faceCards = new string[4] { "A", "K", "Q", "J" };
                for (int i = 0; i < faceCards.Length; i++)
                {
                    deck.Add(faceCards[i] + spade);
                    deck.Add(faceCards[i] + heart);
                    deck.Add(faceCards[i] + diamond);
                    deck.Add(faceCards[i] + club);
                }
            }
            return deck;
        }

        static int GetValueOfCard(string card)
        {
            int value = 0;
            if (card[0] == 'A')
            {
                value = 11;
            }
            else
                if (card[0] >= 50 && card[0] <= 57)
                {
                    value = int.Parse(card[0].ToString());
                }
                else
                {
                    value = 10;
                }
            return value;
        }

        static string DrawCard(List<string> deck)
        {
            Random rng = new Random();
            int rngesus = rng.Next(0, deck.Count);
            if (deck.Count == 0)
            {
                deck = InitialiseDecks();
            }
            string card = deck[rngesus];
            deck.RemoveAt(rngesus);
            Thread.Sleep(20);
            return card;
        }

        static int GetScore(List<string> cards)
        {
            int score = 0;
            int acesCount = 0;
            for (int i = 0; i < cards.Count; i++)
            {
                score += GetValueOfCard(cards[i]);
                if (GetValueOfCard(cards[i]) == 11)
                {
                    acesCount++;
                }
            }
            if (score > 21)
            {
                score -= (10 * acesCount);
            }
            return score;
        }

        static void PrintScore(List<string> playerCards, List<string> dealerCards)
        {
            Random rng = new Random();
            int cardColor;
            int cursorX = 15;
            int cursorY = 15;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition(0, cursorY + 3);
            Console.Write("Dealer cards:");
            Console.ResetColor();
            for (int k = 0; k < dealerCards.Count; k++)
            {
                for (int i = 0; i < 7; i++)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(cursorX, cursorY + i);
                    for (int j = 0; j < 8; j++)
                    {
                        if (i == 0 || i == 6 || j == 0 || j == 7)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                        }
                        Console.Write(" ");
                        Console.BackgroundColor = ConsoleColor.White;

                    }
                    Console.WriteLine();

                }
                cardColor = rng.Next(0, 2);
                if (cardColor == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Thread.Sleep(10);
                Console.SetCursorPosition(cursorX + 3, cursorY + 3);
                Console.Write("{0}", dealerCards[k]);
                cursorX += 10;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(cursorX, cursorY + 3);
            Console.Write(("Score: " + GetScore(dealerCards)));
            Console.ResetColor();

            cursorX = 15;
            cursorY = 25;
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.SetCursorPosition(0, cursorY + 3);
            Console.Write("Player cards:");
            Console.ResetColor();
            for (int k = 0; k < playerCards.Count; k++)
            {
                for (int i = 0; i < 7; i++)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.SetCursorPosition(cursorX, cursorY + i);
                    for (int j = 0; j < 8; j++)
                    {
                        if (i == 0 || i == 6 || j == 0 || j == 7)
                        {
                            Console.BackgroundColor = ConsoleColor.DarkBlue;
                        }
                        Console.Write(" ");
                        Console.BackgroundColor = ConsoleColor.White;

                    }
                    Console.WriteLine();

                }
                cardColor = rng.Next(0, 2);
                if (cardColor == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                Thread.Sleep(10);
                Console.SetCursorPosition(cursorX + 3, cursorY + 3);
                Console.Write("{0}", playerCards[k]);
                cursorX += 10;
            }

            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(cursorX, cursorY + 3);
            Console.Write(("Score: " + GetScore(playerCards)));
            Console.ResetColor();
        }

        static void DrawTable(int chips, int bet)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            for (int i = 0; i < Console.BufferHeight; i++)
            {
                Console.SetCursorPosition(100, i);
                Console.WriteLine('|');
            }
            for (int i = 0; i < 101; i++)
            {
                Console.SetCursorPosition(i, 40);
                Console.Write('-');
            }
            Console.ResetColor();
            Console.SetCursorPosition(105, 30);
            Console.WriteLine("Chips: {0}", chips - bet);
            Console.SetCursorPosition(105, 42);
            Console.WriteLine("Bet: {0}", bet);
        }

    }
}
