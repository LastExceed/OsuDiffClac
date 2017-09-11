using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuDiffClac {
    class Program {
        static void Main(string[] args) {
            if (args.Length > 0) {
                string s = File.ReadAllText(args[0]);
                int startindex = s.IndexOf("[TimingPoints]") + 14;
                int endindex = s.IndexOf("[HitObjects]");

                string timingPointsString = s.Substring(startindex, endindex - startindex).Trim();
                string hitObjectsString = s.Substring(endindex + 12).Trim();
                var beatmap = new Beatmap(timingPointsString, hitObjectsString);

                double hitObjectCount = beatmap.hitObjects.Count;
                Console.WriteLine("notes: " + hitObjectCount);
                double duration = (beatmap.hitObjects.Last().start - beatmap.hitObjects.First().start) / 1000;
                Console.WriteLine("duration " + duration);
                double nps = hitObjectCount / duration;
                Console.WriteLine("average NPS: " + nps);

            } else {
                Console.WriteLine("no input found, drag and drop a .osu file onto OsuDiffClac.exe");
            }
            Console.ReadLine();
        }
    }

    class TimingPoint {
        public int offset;
        public double msPerBeat;

        public TimingPoint(string s) {
            var values = s.Split(new char[] { ',' });
            offset = int.Parse(values[0]);
            msPerBeat = double.Parse(values[1], CultureInfo.InvariantCulture);
        }
    }

    class HitObject {
        public int column;
        public int start;
        public int end;
        public bool isSingle {
            get {
                return end == 0;
            }
        }
        public bool isPaired = false;

        public HitObject(string s) {
            var values = s.Split(new char[] { ',', ':' });
            column = (int.Parse(values[0]) - 64) / 128;
            start = int.Parse(values[2]);
            end = int.Parse(values[5]);
        }
    }

    class Beatmap {
        public LinkedList<TimingPoint> timingPoints = new LinkedList<TimingPoint>();
        public LinkedList<HitObject> hitObjects = new LinkedList<HitObject>();

        public LinkedList<HitObject> leftHandNotes { get { return new LinkedList<HitObject>(hitObjects.Where(x => x.column <= 1)); } }
        public LinkedList<HitObject> rightHandNotes { get { return new LinkedList<HitObject>(hitObjects.Where(x => x.column >= 2)); } }

        public Beatmap(string timingPointsString, string hitObjectsString) {
            foreach (var tp in timingPointsString.Split('\n')) {
                var timingPoint = new TimingPoint(tp);
                if (timingPoint.msPerBeat >= 0) {//skip SVs
                    timingPoints.AddLast(timingPoint); 
                }
            }
            foreach (var ho in hitObjectsString.Split('\n')) {
                hitObjects.AddLast(new HitObject(ho));
            }
        }
    }

    class Difficulty {

        delegate double DoubleLambda(double x);

        public static double InverseMashability(Beatmap beatmap) {
            DoubleLambda mashability = x => Math.Atan(x / 33) / Math.PI + 0.5;
            double score = 1;

            if (beatmap.hitObjects.Count > 0) {

                { // Calculate left hand notes
                    var current = beatmap.leftHandNotes.First;
                    var next = current.Next;
                    var previous = current.Previous; // defaults to null since we're starting with the first element

                    while (next.Next != null) {
                        previous = current;
                        current = next;
                        next = next.Next;

                        var distanceUp = next.Value.start - current.Value.start;
                        var distanceDown = current.Value.start - previous.Value.start;

                        if (distanceDown > distanceUp) {
                            // stuff
                        }
                    }
                }
            }

            return score;
        }
    }
}
