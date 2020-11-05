using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ShippingPort
{
    class Program
    {
        public static List<string> textPort = new List<string>();
        public static Random rnd = new Random();
        public static Boat[,] port = new Boat[2,64];
        public static int rejectedBoat = 0;
        public static string filePath = "Portlog.txt";
        static void Main(string[] args)
        {
            ReadFile();
            while (true)
            {
                NextDay();
                RemoveFromPort();
                CreateBoats();
                FixPrintList();
                PrintPort();
                InfoCount();
                SaveFile();
                Console.ReadKey(true);
            }
        }
        public static void CreateBoats() // I denna metod så slumpas det fram vilka båtar som skall skapas, som sedan kallar på olika betoder beroende på båt som sedan skapar.
        {
            for (int i = 0; i < 5; i++)
            {
                switch (rnd.Next(1,5))
                {
                    case 1:
                        CreateRowBoat();
                        break;
                    case 2:
                        CreateMotorBoat();
                        break;
                    case 3:
                        CreateSailingBoat();
                        break;
                    case 4:
                        CreateCargoBoat();
                        break;
                }
            }
        }
        public static void CreateRowBoat()
        {
            string id = MakeRandomId("R-");
            Boat b = new RowBoat(id, rnd.Next(100,301), rnd.Next(0,4), rnd.Next(1,7));
            AddInPort(b);
        }
        public static void CreateMotorBoat()
        {
            string id = MakeRandomId("M-");
            Boat b = new MotorBoat(id, rnd.Next(200, 3001), rnd.Next(0, 61), rnd.Next(10, 1001));
            AddInPort(b);
        }
        public static void CreateSailingBoat()
        {
            string id = MakeRandomId("S-");
            Boat b = new SailingBoat(id, rnd.Next(800, 6001), rnd.Next(0, 13), rnd.Next(10, 61));
            AddInPort(b);
        }
        public static void CreateCargoBoat()
        {
            string id = MakeRandomId("C-");
            Boat b = new CargoShip(id, rnd.Next(3000, 20001), rnd.Next(0, 21), rnd.Next(0, 501));
            AddInPort(b);
        }
        public static string MakeRandomId(string boatType)
        {
            string alf = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string id;

            do
            {
                id = $"{boatType}{alf[rnd.Next(0,alf.Length)]}{alf[rnd.Next(0, alf.Length)]}{alf[rnd.Next(0, alf.Length)]}";

            } while (IdEqualsOrNot(id));

            return id;
        }
        public static bool IdEqualsOrNot(string id)
        {
            foreach (var boat in port)
            {
                if (boat != null && id == boat.IdName)
                {
                    return true;
                }
            }
            return false;
        }
        public static void AddInPort(Boat b)
        {
            bool addOrNot = true;
            bool notAdded = true;

            if (b is RowBoat)  // Börjar med att leta upp en rowboat för att se om den sedan har en plats ledig på samma "plats"
            {
                for (int i = 0; i < 64; i++)
                {
                    if (port[0,i] is RowBoat)
                    {
                        if (port[1, i] == null)
                        {
                            port[1, i] = b;
                            addOrNot = false;
                            notAdded = false;
                            break;
                        }
                    }
                }
                if (addOrNot) // Använder denna ifsats ifall den har hittat en annan rowboat, fast den inte hade en ledig plats på samma "plats"
                {
                    for (int k = 0; k < 64; k++)
                    {
                        if (port[0, k] == null)
                        {
                            port[0, k] = b;
                            notAdded = false;
                            break;
                        }
                    }
                }
            }
            else if (b is MotorBoat)
            {
                for (int i = 0; i < 64; i++)
                {
                    if (port[0, i] == null)
                    {
                        port[0, i] = b;
                        notAdded = false;
                        break;
                    }
                }
            }
            else if (b is SailingBoat)
            {
                for (int i = 0; i < 63; i++)
                {
                    if (port[0, i] == null && port[0, i+1] == null)
                    {
                        port[0, i] = b;
                        port[0, i+1] = b;
                        notAdded = false;
                        break;
                    }
                }
            }
            else if (b is CargoShip)  // För Cargoship så har jag valt att köra en forloop baklänges för att placera dessa objekt längst ner i min hamn.
            {
                for (int i = 63; i > 3; i--)
                {
                    if (port[0, i] == null && port[0, i-1] == null && port[0, i-2] == null && port[0, i-3] == null)
                    {
                        port[0, i] = b;
                        port[0, i-1] = b;
                        port[0, i-2] = b;
                        port[0, i-3] = b;
                        notAdded = false;
                        break;
                    }
                }
            }
            if (notAdded)
            {
                rejectedBoat++;
            }
        }
        public static void NextDay() // Används för att sänka värdet på varje objekts "DayInPort" prop.
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    if (port[i, j] != null)
                    {
                        port[i, j].DaysInPort--;
                        j += port[i, j].BoatLength - 1;
                    }
                }
            }
        }
        public static void RemoveFromPort()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    if (port[i,j] != null)
                    {
                        if (port[i,j].DaysInPort == 0)
                        {
                            port[i, j] = null;
                        }
                    }
                }
            }
        }
        public static void FixPrintList() // Denna metod gör om min 2D array till en list och ser till att allt kommer i rätt ordning visuelt, bland annat så man inte skriver ut de övriga lediga Halvplatserna.
        {
            textPort.Clear();
            Console.Clear();

            Console.WriteLine($"Spot\tBoattype\tID-Nr\tWeight\tTopspeed\tMisc");
            Console.WriteLine("");

            for (int j = 0; j < 64; j++)
            {
                if (port[0, j] == null)
                {
                     textPort.Add($"{j + 1}\tEmpty");
                }
                else if (port[0, j] is RowBoat)
                {
                    RowBoat b0 = port[0, j] as RowBoat;
                    textPort.Add($"{j + 1}\tRowboat\t\t{port[0, j].IdName}\t{port[0, j].Weight}\t{KnopToKmh(port[0, j].TopSpeed)} KmH\t\t{b0.MaxPassenger}");
                    if (port[1, j] is RowBoat)
                    {
                        RowBoat b1 = port[1, j] as RowBoat;
                        textPort.Add($"{j + 1}\tRowboat\t\t{port[1, j].IdName}\t{port[1, j].Weight}\t{KnopToKmh(port[1, j].TopSpeed)} KmH\t\t{b1.MaxPassenger}");
                    }
                }
                else if (port[0, j] is MotorBoat)
                {
                    MotorBoat b = port[0, j] as MotorBoat;
                    textPort.Add($"{j + 1}\tMotorboat\t{port[0, j].IdName}\t{port[0, j].Weight}\t{KnopToKmh(port[0, j].TopSpeed)} KmH\t\t{b.HorsePower}");
                }
                else if (port[0, j] is SailingBoat)
                {
                    SailingBoat b = port[0, j] as SailingBoat;
                    textPort.Add($"{j + 1}-{j + 2}\tSailingboat\t{port[0, j].IdName}\t{port[0, j].Weight}\t{KnopToKmh(port[0, j].TopSpeed)} KmH\t\t{b.BoatLength}");
                    j += 1;
                }
                else if (port[0, j] is CargoShip)
                {
                    CargoShip b = port[0, j] as CargoShip;
                    textPort.Add($"{j + 1}-{j + 4}\tCargoship\t{port[0, j].IdName}\t{port[0, j].Weight}\t{KnopToKmh(port[0, j].TopSpeed)} KmH\t\t{b.NumberOfContainers}");
                    j += 3;
                }
            }
        }
        public static double KnopToKmh(int knop)
        {
            double kmh = knop * 1.85;
            return (int)kmh;
        }
        public static void PrintPort()
        {
            foreach (var b in textPort)
            {
                Console.WriteLine(b);
            }
            Console.WriteLine("");
        }
        public static void InfoCount() // Räknar ut övrig information om vad som finns i hamnen och skriver ut den.
        {
            int countRowBoat = 0;
            int countMotorBoat = 0;
            int countSailingBoat = 0;
            int countCargoShip = 0;
            int numberOfBoats = 0;
            int totalWeight = 0;
            int freeSpots = 0;
            int totalMaxSpeed = 0;
            int averageMaxSpeed = 0;

            for (int i = 0; i < 64; i++)
            {
                if (port[0,i] is RowBoat)
                {
                    countRowBoat++;
                    totalWeight += port[0, i].Weight;
                    totalMaxSpeed += port[0, i].TopSpeed;

                    if (port[1, i] is RowBoat)
                    {
                        countRowBoat++;
                        totalWeight += port[1, i].Weight;
                        totalMaxSpeed += port[0, i].TopSpeed;
                    }
                }
                else if (port[0,i] is MotorBoat)
                {
                    countMotorBoat++;
                    totalWeight += port[0, i].Weight;
                    totalMaxSpeed += port[0, i].TopSpeed;
                }
                else if (port[0, i] is SailingBoat)
                {
                    countSailingBoat++;
                    totalWeight += port[0, i].Weight;
                    totalMaxSpeed += port[0, i].TopSpeed;
                    i += 1;
                }
                else if (port[0, i] is CargoShip)
                {
                    countCargoShip++;
                    totalWeight += port[0, i].Weight;
                    totalMaxSpeed += port[0, i].TopSpeed;
                    i += 3;
                }
                else if (port[0, i] is null)
                {
                    freeSpots++;
                }
            }
            numberOfBoats = countRowBoat + countMotorBoat + countSailingBoat + countCargoShip;
            averageMaxSpeed = totalMaxSpeed / numberOfBoats;
            Console.WriteLine($"Rowboats: {countRowBoat}. Motorboats: {countMotorBoat} Sailingboats: {countSailingBoat}. Cargoship: {countCargoShip}. Freespots: {freeSpots}.");
            Console.WriteLine($"Total Weight: {totalWeight}. Average Topspeed: {KnopToKmh(averageMaxSpeed)}Km/h");
            Console.WriteLine($"Rejected Boats: {rejectedBoat}");
        }
        public static void ReadFile() // läser av txt filen och skapar om objekten och sedan placerar dessa i rätt index i min 2D array.
        {
            if (File.Exists(filePath))
            {
                List<string> lines = File.ReadAllLines(filePath).ToList();

                foreach (var line in lines)
                {
                    string[] entries = line.Split(',');

                    if (entries[2] == "RowBoat")
                    {
                        Boat b = new RowBoat(entries[3], int.Parse(entries[4]), int.Parse(entries[5]), int.Parse(entries[6]), int.Parse(entries[7]));
                        port[int.Parse(entries[0]), int.Parse(entries[1])] = b;
                    }
                    else if (entries[2] == "MotorBoat")
                    {
                        Boat b = new MotorBoat(entries[3], int.Parse(entries[4]), int.Parse(entries[5]), int.Parse(entries[6]), int.Parse(entries[7]));
                        port[int.Parse(entries[0]), int.Parse(entries[1])] = b;
                    }
                    else if (entries[2] == "SailingBoat")
                    {
                        Boat b = new SailingBoat(entries[3], int.Parse(entries[4]), int.Parse(entries[5]), int.Parse(entries[6]), int.Parse(entries[7]));
                        port[int.Parse(entries[0]), int.Parse(entries[1])] = b;
                    }
                    else if (entries[2] == "CargoShip")
                    {
                        Boat b = new CargoShip(entries[3], int.Parse(entries[4]), int.Parse(entries[5]), int.Parse(entries[6]), int.Parse(entries[7]));
                        port[int.Parse(entries[0]), int.Parse(entries[1])] = b;
                    }
                }
            }
        }
        public static void SaveFile()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            List<string> output = new List<string>();

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 64; j++)
                {
                    if (port[i,j] != null)
                    {
                        output.Add($"{i},{j},{port[i, j].GetType().Name},{port[i, j].IdName},{port[i, j].Weight},{port[i, j].TopSpeed},{port[i,j].GetUniqueProp()},{port[i,j].DaysInPort}");
                    }
                }
            }
            File.WriteAllLines(filePath, output);
        }
    }
}
