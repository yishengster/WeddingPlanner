using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeddingPlanner
{
    public class WeddingTable
    {
        string _name;
        int _capacity;
        int _freSpots;
        List<Family> _guestList = new List<Family> { };
        public WeddingTable(string name, int capacity)
        {
            _name = name;
            _capacity = capacity;
        }
        public string Name
        {
            get { return this._name; }
        }

        public int Capacity
        {
            get { return this._capacity; }
        }
        public int FreeSpots
        {
            get
            {
                if (GuestList == null)
                    return this._capacity;
                else
                    return _capacity - GuestList.Sum(x => x.Size);
            }
            set { _freSpots = value; }
        }
        public List<Family> GuestList
        {
            get { return this._guestList; }
        }
        public string FamilyNames
        {
            get { return String.Join(",", _guestList.Select(z => z.Name)); }
        }
        public void AddGestList(Family fmly)
        {
            _guestList.Add(fmly);
        }
        public void RemoveGestList(Family fmly)
        {
            _guestList.Remove(fmly);
        }
        public bool FirendlyWith(string name)
        {
            string nameList = String.Join(",", _guestList.Select(z => z.Name));
            if (nameList.Contains(name))
                return false;
            else
                return true;
        }
    }
}
