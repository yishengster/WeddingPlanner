using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeddingPlanner
{
    public class Family
    {
        string _name;
        int _size;
        string _dislike;
        string _tableName;

        public Family(string name, int size, string dislike)
        {
            _name = name;
            _size = size;
            _dislike = dislike;
        }
        public string Name {
            get { return this._name; }   
        }
        public int Size
        {
            get { return this._size; }
        }
        public string Dislike
        {
            get { return this._dislike; }
        }
        public string TableName
        {
            get { return this._tableName; }
            set { this._tableName = value; }
        }
        public bool Likes(string name)
        {
            if (string.IsNullOrEmpty(_dislike))
                return true;

            if (name.Contains(_dislike))
                return false;
            else
                return true;
        }
    }
}
