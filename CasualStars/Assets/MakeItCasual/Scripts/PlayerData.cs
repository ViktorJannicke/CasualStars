
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
