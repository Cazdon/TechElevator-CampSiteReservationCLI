using Capstone.DAO;
using Capstone.Models;
using System;
using System.Collections.Generic;

namespace Capstone
{
    public class CapstoneCLI
    {
        #region Member Variables
        private string _connectionString;
        #endregion

        #region Constructor
        public CapstoneCLI(string connectionString)
        {
            _connectionString = connectionString;
        }
        #endregion


        public void MainMenu()
        {
                ViewParksInterface();
        }

        private void ViewParksInterface()
        {
            bool exit = false;

            
            while (!exit)
            {
                Console.Clear();
                ParkDAO dao = new ParkDAO(_connectionString);
                IList<Park> parks = dao.GetParks();

                //Dictionary created for passing into ParkInformationInterface Method
                Dictionary<int, Park> parkDict = new Dictionary<int,Park>();

                if (parks.Count > 0)
                {
                    Console.WriteLine("Select a Park for Further Details...");
                    foreach (Park p in parks)
                    {
                        Console.WriteLine($"{p.ParkId}) {p.Name.PadRight(8)}");
                        parkDict.Add(p.ParkId, p);
                    }
                    Console.WriteLine($"{parks.Count+1}) Exit");
                    int selection = CLIHelper.GetSingleInteger("Selection...", 1, parks.Count + 1);

                    if (selection == parks.Count + 1)
                    {
                        exit = true;
                    }
                    else
                    {
                        ParkInformationInterface(parkDict, selection);
                    }
                }

                else
                {
                    Console.Clear();
                    Console.WriteLine("****** NO RESULTS ******");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    exit = true;
                }
            }
        }

        private void ParkInformationInterface(Dictionary<int, Park> parkDictionary, int selection)
        {
            bool exit = false;
            Park selectedPark = parkDictionary[selection];

            while (!exit)
            {
               
                Console.Clear();
                Console.WriteLine($"{selectedPark.Name} National Park");
                Console.WriteLine("Location: ".PadRight(18) + $"{selectedPark.Location}");
                Console.WriteLine("Established: ".PadRight(18) + $"{selectedPark.EstablishDate.ToString("d")}");
                Console.WriteLine("Area: ".PadRight(18) + $"{selectedPark.Area.ToString()}");
                Console.WriteLine("Annual Visitors: ".PadRight(18) + $"{selectedPark.Visitors.ToString()}");
                Console.WriteLine();
                Console.WriteLine(selectedPark.Description); //Maybe write a method for formatting.
                Console.WriteLine();

                Console.WriteLine("Select a Command");
                Console.WriteLine("1) View Campgrounds");
                Console.WriteLine("2) Search for Reservation");
                Console.WriteLine("3) Return to Previous Screen");

                int choice = CLIHelper.GetSingleInteger("Selection...", 1, 3);

                if (choice == 1)
                {
                    ViewCampsInterface(selectedPark);
                }

                if (choice == 2)
                {
                    Console.Clear();
                    Console.WriteLine("Not implemented yet :^) ");
                    Console.ReadKey();
                    //Bonus number 5
                }

                if (choice == 3)
                {
                    exit = true;
                }
            }
        }

        private void ViewCampsInterface(Park selectedPark)
        {
            bool exit = false;

            while (!exit)
            {
                Console.Clear();
                CampgroundDAO dao = new CampgroundDAO(_connectionString);                
                IList<Campground> camps = dao.GetCampgrounds(selectedPark);
                int Campcount = 0;

                //Dictionary created for passing into Infromation into other Methods
                Dictionary<int, Campground> campDict = new Dictionary<int, Campground>();

                if (camps.Count > 0)
                {
                    Console.WriteLine($"{selectedPark.Name} National Park Campgrounds");
                    Console.WriteLine();
                    Console.WriteLine("".PadRight(3) + "Name".PadRight(35) + "Open".PadRight(18) + "Close".PadRight(18) + "Daily Fee");
                    foreach (Campground c in camps)
                    {                        
                        Console.WriteLine($"#{++Campcount} {c.Name.PadRight(35)} " +
                                          $"{c.OpenFromString.PadRight(18)} {c.OpenToString.PadRight(18)} {c.DailyFee.ToString("C")}");

                        campDict.Add(Campcount, c);
                    }
                    Console.WriteLine();
                    Console.WriteLine("Select a Command");
                    Console.WriteLine("1) Search for Available Reservation");
                    Console.WriteLine("2) Return to Previous Screen");

                    int selection = CLIHelper.GetSingleInteger("Selection... ", 1, 2);
                    Console.WriteLine();  

                    if (selection == 1)
                    {
                        Console.WriteLine("Which campground? (enter 0 to cancel)");
                        int choice = CLIHelper.GetSingleInteger("Selection... ", 0, camps.Count);
                        Console.WriteLine();

                        if (choice == 0)
                        {
                            //Blank to just loop back.
                        }
                        else
                        {
                            SearchForReservationInterface(campDict[choice]);
                        }
                    }
                    else if (selection == 2)
                    {
                        exit = true;
                    }
                }

                else
                {
                    Console.Clear();
                    Console.WriteLine("****** NO RESULTS ******");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    exit = true;
                }
            }
        }

        private void SearchForReservationInterface(Campground selectedCamp)
        {
            bool exit = false;

            while (!exit)
            {
                Console.WriteLine("What is the arrival date? ");
                DateTime arrivalDate = CLIHelper.GetDateTime("Enter in year-month-day format.");

                Console.WriteLine("What is the departure date?");
                DateTime depatureDate = CLIHelper.GetDateTime("Enter in year-month-day format.");

                SiteDAO dao = new SiteDAO(_connectionString);
                try
                {
                    IList<Site> sites = dao.GetTop5Sites(selectedCamp, arrivalDate, depatureDate);



                    if (sites.Count > 0)
                    {
                        Console.Clear();

                        Console.WriteLine($"Results Matching Your Search Criteria");
                        Console.WriteLine("Site No.".PadRight(15) + "Max Occup.".PadRight(15) +
                                          "Accessible?".PadRight(15) + "Max RV Length".PadRight(15) +
                                          "Utility".PadRight(15) + "Cost");

                        Dictionary<int, Site> siteDict = new Dictionary<int, Site>();
                        int siteCount = 0;


                        foreach (Site s in sites)
                        {
                            Console.WriteLine($"{++siteCount}) " +
                                              $"{s.SiteNumber.ToString().PadRight(15)}" +
                                              $"{s.MaxOccupancy.ToString().PadRight(15)}" +
                                              $"{s.AccessibleString.PadRight(15)}" +
                                              $"{s.MaxRVString.PadRight(15)}" +
                                              $"{s.UtilitiesString.PadRight(15)}" +
                                              $"{selectedCamp.DailyFee.ToString("C")}");


                            siteDict.Add(siteCount, s);
                        }
                        Console.WriteLine();
                        int selection = CLIHelper.GetSingleInteger("Which site should be reserved(enter 0 to cancel) ", 0, sites.Count);
                        Console.WriteLine();

                        if (selection == 0)
                        {
                            exit = true;
                        }
                        else
                        {
                            //ReservationDAO  = new ReservationDAO(_connectionString);
                            string name = CLIHelper.GetString("What name should the reservation be made under? ");
                            MakeReservationInterface(siteDict[selection], name, arrivalDate, depatureDate);
                            exit = true;
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Console.WriteLine("****** NO RESULTS ******");
                        Console.WriteLine("");
                        Console.ReadKey();
                        exit = true;
                    }
                }
                catch 
                {
                    Console.Clear();
                    Console.WriteLine("Something went wrong with the Dates you entered... try again.");
                }
            }
        }

        private void MakeReservationInterface(Site site, string name, DateTime arrivalDate, DateTime departureDate)
        {
            ReservationDAO dao = new ReservationDAO(_connectionString);
            int confirmationId = dao.MakeReservation(site, name, arrivalDate, departureDate);

            Console.WriteLine($"The reservation has been made and the confirmation id is {confirmationId.ToString()}");
            Console.WriteLine($"Press any key to return to the previous screen...");
            Console.ReadKey();
        }
    }
}
