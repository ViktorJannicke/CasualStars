
    [System.Serializable]
    public class PlayerData
    {
        public int id;
        public string name;

        //Scoreboard
        public string score;

        public PlayerData(int _id, string _name, string _score)
        {
            id = _id;
            name = _name;
            score = _score;
        }
}
