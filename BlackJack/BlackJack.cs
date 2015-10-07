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
                Console.WriteLine("You had {0} wins, {1} ties and {2} losses by now", winCount, tieCount, loseCount);
            }
            if (chips == 0)
            {
                Console.WriteLine("You have 0 chips, do you want to start over ? (yes/no)");

                while (true)
                {
                    string choice = Console.ReadLine();
                    if (choice == "yes")
                    {
                        chips = 1000;
                        break;
                    }
                    if (choice == "no")
                    {
                        Console.WriteLine("Good bye !");
                        return;
                    }
                    if (choice != "yes" || choice != "no")
                    {
                        Console.WriteLine("Invalid input, please type yes or no.");
                    }
                }
            }
            List<string> deck = InitialiseDecks();
            while (chips > 0)
            {
                List<string> dealerCards = new List<string>();
                List<string> playerCards = new List<string>();
                Console.WriteLine("Enter the amount of chips you want to bet (you have {0} chips)", chips);
                int bet = int.Parse(Console.ReadLine());
                bool win = true;
                bool gameover = false;

                dealerCards.Add(DrawCard(deck));
                Console.WriteLine(string.Join(", ", dealerCards));
                Console.WriteLine(GetScore(dealerCards));


                playerCards.Add(DrawCard(deck));
                playerCards.Add(DrawCard(deck));
                Console.WriteLine(string.Join(", ", playerCards));
                Console.WriteLine(GetScore(playerCards));


                while (true)
                {
                    Console.WriteLine("Hit or stand");
                    string choice = Console.ReadLine().ToLower();
                    if (choice == "h" || choice =="hit")
                    {

                        playerCards.Add(DrawCard(deck));
                        Console.WriteLine(string.Join(", ", playerCards));
                        Console.WriteLine(GetScore(playerCards));
                    }
                    if (GetScore(playerCards) == 21)
                    {
                        Console.WriteLine("Blackjack!");
                        win = true;
                        gameover = true;
                        tieCount++;
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
                    if (choice == "s" || choice=="stand")
                    {

                        while (true)
                        {
                            if (GetScore(dealerCards) <= 16)
                            {
                                Console.WriteLine("Dealer hits");
                                dealerCards.Add(DrawCard(deck));
                                Console.WriteLine(string.Join(", ", dealerCards));
                                Console.WriteLine(GetScore(dealerCards));
                            }
                            else if (GetScore(dealerCards) >= 17 && GetScore(dealerCards) <= 20)
                            {
                                Console.WriteLine("Dealer stands");
                                Console.WriteLine(string.Join(", ", dealerCards));
                                Console.WriteLine(GetScore(dealerCards));
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
                        Console.WriteLine("Player wins");
                        chips += bet;
                    }
                    else
                    {
                        Console.WriteLine("Dealer wins");
                        chips -= bet;
                    }
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
    }
}