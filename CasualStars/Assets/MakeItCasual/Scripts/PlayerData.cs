using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    [System.Serializable]
    public class PlayerData
    {

        public string name;

        public int health;
        public int shield;

        //Scoreboard
        public int score;

        //Shop
        public int credits;

        public PlayerData(string _name, int _health, int _shield, int _score, int _credits)
        {

            name = _name;
            health = _health;
            shield = _shield;
            score = _score;
            credits = _credits;

        }
    }
