
    [System.Serializable]
    public class PlayerData
    {
        public int id;
        public string name;

        //Scoreboard
        public int score;

        public PlayerData(int _id, string _name, int _score)
        {
            id = _id;
            name = _name;
            score = _score;
        }
}
