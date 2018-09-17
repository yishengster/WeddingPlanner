using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeddingPlanner
{
    class Program
    {
        static List<WeddingTable> _tableList;
        static List<Family> _unseated_list, _leftover_list;
        static void Main()
        {
            Family Welters = new Family("Welters", 6, "");
            Family Smiths = new Family("Smiths", 4, "Welters");
            Family Burks = new Family("Burks", 3, "");
            Family Leskos = new Family("Leskos", 2, "");
            //Family Leskos = new Family("Leskos", 2, "Burks"); //involving two moves: Smiths switches with Burks, and Adding Leskos to table A. The program is yet too dumb to do that. 
            _unseated_list = new List<Family> { Welters, Smiths, Leskos, Burks };

            WeddingTable tableA = new WeddingTable("A", 8);
            WeddingTable tableB = new WeddingTable("B", 6);
            WeddingTable tableC = new WeddingTable("C", 4);
            _tableList = new List<WeddingTable> { tableA, tableB, tableC };
            _leftover_list = new List<Family> { };

            if (_unseated_list.Sum(x => x.Size) > _tableList.Sum(x => x.Capacity))
            {
                Console.WriteLine("You need more tables");
                Console.Read();
                return;
            }

            //largest family first
            _unseated_list = _unseated_list.OrderByDescending((x => x.Size)).ToList();
            //smallest table first, so that we can assign the current largest family to 
            //the first 
            _tableList = _tableList.OrderBy((x => x.Capacity)).ToList();
            while (_unseated_list.Count > 0)
            {
                //try my luck...
                try
                {
                    Assign_Seats(_unseated_list[0]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.Read();
                    return;
                }
            }

            //all is good
            if (_leftover_list.Count == 0)
            {
                //print all the stuff
                PrintResults();
                return;
            }
            else
            {
                //try BASIC switch, print result if lucky
                while (_leftover_list.Count > 0)
                {
                    //try to switch seats
                    if (!SwitchSeats(_leftover_list[0]))
                    {
                        Console.WriteLine("Some families cannot be seated. Or the program isn't yet smart enough.");
                        Console.Read();
                        return;
                    }
                }

                //luck strikes! 
                PrintResults();
            }
        }
        /// <summary>
        /// Initial assignment
        /// </summary>
        /// <param name="fmly"></param>
        private static void Assign_Seats(Family fmly)
        {
            bool assigned = false; //always first
            //tableList = tableList.OrderByDescending(y => y.FreeSpots).ToList();
            for (int j = 0; j < _tableList.Count; j++)
            {
                //initial assignment
                if (_tableList[j].FreeSpots >= fmly.Size
                    && _tableList[j].FirendlyWith(fmly.Name)
                    && fmly.Likes(_tableList[j].FamilyNames))
                {
                    fmly.TableName = _tableList[j].Name;
                    _tableList[j].GuestList.Add(fmly);
                    _unseated_list.Remove(fmly);
                    assigned = true;
                    break;
                }
            }
            if (!assigned)
            {
                //determine whether it's because of table capacity
                int largest_spot = _tableList.Max(x => x.FreeSpots);
                if (largest_spot < _unseated_list[0].Size)
                {
                    throw new Exception("Not enough large table to sit enire families.");
                }

                _leftover_list.Add(_unseated_list[0]);
                _unseated_list.Remove(_unseated_list[0]);
            }

        }
        private static void PrintResults()
        {
            //sort by table name
            _tableList = _tableList.OrderBy(x => x.Name).ToList();
            foreach (var x in _tableList)
            {
                string tableName = "Table " + x.Name;
                string family = string.Empty;
                foreach (var y in x.GuestList)
                {
                    family += y.Name + " " + "Party of " + y.Size.ToString() + ". ";
                }
                Console.WriteLine(tableName + ": " + family);
            }
            Console.Read();
        }

        #region Unfinished work for switching
        /// <summary>
        /// Starting some effort on switching - no time to work out a good solution yet. 
        /// </summary>
        /// <param name="leftOver"></param>
        /// <returns></returns>
        private static bool SwitchSeats(Family leftOver)
        {
            //build target list
            List<Family> switchList = BuildSwitchTargeList(leftOver);
            //go through the list and trying to find a spot from the EXISTING table list
            foreach (var x in switchList)
            {
                if (Assign_Switched_Seats(x))
                {
                    //do switch
                    foreach (var y in _tableList)
                    {
                        if (y.Name == x.TableName)
                        {
                            y.GuestList.Remove(x);
                            y.GuestList.Add(leftOver);
                            break;
                        }
                    }
                    _leftover_list.Remove(leftOver);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// trying to find the switch target a spot
        /// </summary>
        /// <param name="fmly"></param>
        /// <returns></returns>
        private static bool Assign_Switched_Seats(Family fmly)
        {
            bool assigned = false; //always first
            //tableList = tableList.OrderByDescending(y => y.FreeSpots).ToList();
            for (int j = 0; j < _tableList.Count; j++)
            {
                //initial assignment
                if (_tableList[j].FreeSpots >= fmly.Size
                    && _tableList[j].FirendlyWith(fmly.Name)
                    && fmly.Likes(_tableList[j].FamilyNames))
                {
                    fmly.TableName = _tableList[j].Name;
                    _tableList[j].GuestList.Add(fmly);
                    _leftover_list.Remove(fmly);
                    assigned = true;
                    break;
                }
            }
            return assigned;
        }
        /// <summary>
        /// Go through the current table list to determine switch targets
        /// The target can be one family, and can be a combination of families,
        /// currently sitting on the table, whose combined sizes 
        /// is suitable for a switch.  
        /// Cannot do two moves yet.
        /// For simplicity, we just look for a single family at a time
        /// for switching. No combinations. 
        /// </summary>
        /// <param name="fmly"></param>
        /// <returns></returns>
        private static List<Family> BuildSwitchTargeList(Family fmly)
        {
            List<Family> list = new List<Family> { };

            for (int j = 0; j < _tableList.Count; j++)
            {
                //only occupied tables
                if (_tableList[j].GuestList != null)
                {
                    for (int k = 0; k < _tableList[j].GuestList.Count; k++)
                    {
                        WeddingTable currentTable = _tableList[j];
                        Family currentGuest = _tableList[j].GuestList[k];
                        if (fmly.Size <= currentGuest.Size + _tableList[j].FreeSpots)
                        {
                            //check if the family is compatible with the remaining families
                            //if so, add
                            currentTable.GuestList.Remove(currentGuest);
                            if (currentTable.FirendlyWith(fmly.Name) && fmly.Likes(currentTable.FamilyNames))
                                list.Add(currentGuest);
                            //add it back
                            currentTable.GuestList.Add(currentGuest);
                        }
                    }
                }
            }

            return list;
        }
        /// <summary>
        /// Not used. From the Staock Overflow. 
        /// For hypothetical improvement to the current task
        /// </summary>
        /// <param name="list"></param>
        //static List<string> GetCombination(List<int> list)
        //{
        //    string result = string.Empty;
        //    List<string> retList = new List<string> { };
        //    double count = Math.Pow(2, list.Count);
        //    for (int i = 1; i <= count - 1; i++)
        //    {
        //        string str = Convert.ToString(i, 2).PadLeft(list.Count, '0');
        //        for (int j = 0; j < str.Length; j++)
        //        {
        //            if (str[j] == '1')
        //            {
        //                Console.Write(list[j]);
        //                result += list[j].ToString();
        //            }
        //        }
        //        Console.WriteLine();
        //        retList.Add(result);
        //        result = string.Empty;
        //    }

        //    return retList;
        //}
    }
        #endregion
}
