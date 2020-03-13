using System;


namespace POT.WorkingClasses
{
    public class WorksISS
    {
        private String hrv;
        private String eng;
        private String id;

        public WorksISS() { }

        public void CopyFrom(WorksISS _wss)
        {
            this.Hrv = _wss.Hrv;
            this.Eng = _wss.Eng;
            this.Id = _wss.Id;
        }

        public string Hrv { get => hrv; set => hrv = value; }
        public string Eng { get => eng; set => eng = value; }
        public string Id { get => id; set => id = value; }
    }
}
