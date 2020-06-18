using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    [System.Serializable]
    public class PlayerData
    {

        public string name;

        //Scoreboard
        public int score;

        public PlayerData(string _name, int _score)
        {
            name = _name;
            score = _score;
        }
}
