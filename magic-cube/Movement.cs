using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace magic_cube {
    public enum Move {
        //clockwise
        F=-9, R, B, L, U, D,
        M, //Middle: the layer between L and R
        E, //Equator: the layer between U and D
        S, //Standing: the layer between F and B
        None,
        //counter-clockwise
        Fp, Rp, Bp, Lp, Up, Dp, Mp, Ep, Sp //p from prime: '
    }

    public enum SwipeDirection {
        None,
        H, //horizontal
        V, //vertical
    }

    public struct SwipedFace {
        public CubeFace face;
        public SwipeDirection direction;
        public int layer;

        public SwipedFace(CubeFace f, SwipeDirection direction, int layer) {
            this.face = f;
            this.direction = direction;
            this.layer = layer;
        }
    }

    public class Movement {
        private HashSet<string> touchedFaces;
        public HashSet<string> TouchedFaces {
            get{
                return this.touchedFaces;
            }

            set {
                this.touchedFaces = value;
                swipedFaces.Clear();
                parse();
            }
        }

        public List<SwipedFace> swipedFaces = new List<SwipedFace>();

        public Movement() { }

        public Movement(HashSet<string> touchedFaces) {
            this.touchedFaces = touchedFaces;
        }

        private void parse() {
            foreach (string tf in touchedFaces) {
                CubeFace face = (CubeFace)Enum.Parse(typeof(CubeFace), tf[0].ToString());
                SwipeDirection dir = (SwipeDirection)Enum.Parse(typeof(SwipeDirection), tf[1].ToString());
                int layer = Convert.ToInt32(tf[2].ToString());

                swipedFaces.Add(new SwipedFace(face, dir, layer));
            }
        }

        public Move getMove() {
            Move retval = Move.None;
            CubeFace f = getDominantFace();

            if (f == CubeFace.None) {
                return retval;
            }

            filterMoves(f);
            SwipeDirection dir = getSingleDirection();

            if (dir == SwipeDirection.None) {
                return retval;
            }

            Debug.Print(dir.ToString());

            SwipedFace swipedFace = getSingleSwipedFace(dir);
        
            Debug.Print("face: {0}{1}{2}", swipedFace.face, swipedFace.direction, swipedFace.layer);

            switch(swipedFace.face){
                case CubeFace.F: case CubeFace.B:
                    switch(swipedFace.direction){
                        case SwipeDirection.H:
                            switch(swipedFace.layer){
                                case 0:
                                    retval = Move.U;
                                    break;
                                case 1:
                                    retval = Move.E;
                                    break;
                                case 2:
                                    retval = Move.D;
                                    break;
                            }
                            break;
                        case SwipeDirection.V:
                            switch (swipedFace.layer) {
                                case 0:
                                    retval = Move.L;
                                    break;
                                case 1:
                                    retval = Move.M;
                                    break;
                                case 2:
                                    retval = Move.R;
                                    break;
                            }
                            break;
                    }
                    break;
                case CubeFace.R: case CubeFace.L:
                    switch (swipedFace.direction) {
                        case SwipeDirection.H:
                            switch(swipedFace.layer){
                                case 0:
                                    retval = Move.D;
                                    break;
                                case 1:
                                    retval = Move.E;
                                    break;
                                case 2:
                                    retval = Move.U;
                                    break;
                            }
                            break;
                        case SwipeDirection.V:
                            switch (swipedFace.layer) {
                                case 0:
                                    retval = Move.B;
                                    break;
                                case 1:
                                    retval = Move.S;
                                    break;
                                case 2:
                                    retval = Move.F;
                                    break;
                            }
                            break;
                    }
                    break;
                case CubeFace.U: case CubeFace.D:
                    switch (swipedFace.direction) {
                        case SwipeDirection.H:
                            switch (swipedFace.layer) {
                                case 0:
                                    retval = Move.B;
                                    break;
                                case 1:
                                    retval = Move.S;
                                    break;
                                case 2:
                                    retval = Move.F;
                                    break;
                            }
                            break;
                        case SwipeDirection.V:
                            switch (swipedFace.layer) {
                                case 0:
                                    retval = Move.L;
                                    break;
                                case 1:
                                    retval = Move.M;
                                    break;
                                case 2:
                                    retval = Move.R;
                                    break;
                            }
                            break;
                    }
                    break;
            }

            Debug.Print("Move: " + retval.ToString());

            return retval;
        }

        private SwipedFace getSingleSwipedFace(SwipeDirection dir) {
            return swipedFaces.Where(x => x.direction == dir).First();
        }

        private SwipeDirection getSingleDirection(){
            Dictionary<SwipeDirection, int> directionCount = new Dictionary<SwipeDirection, int>() {
                {SwipeDirection.H, 0},
                {SwipeDirection.V, 0},
            };

            foreach (var s in swipedFaces) {
                directionCount[s.direction]++;
            }

            try {
                return directionCount.Where(count => count.Value == 1).First().Key;
            }
            catch(InvalidOperationException ex){
                return SwipeDirection.None;
            }
        }

        private CubeFace getDominantFace(){
            Dictionary<CubeFace, int> faceCount = new Dictionary<CubeFace,int>();
            int count;
            foreach(var f in swipedFaces){
                count = 0;
                faceCount.TryGetValue(f.face, out count);
                faceCount[f.face] = ++count;
            }

            CubeFace dominantFace = new CubeFace();
            int max = Int32.MinValue;
            foreach (var i in faceCount) {
                if (i.Value > max) {
                    max = i.Value;
                    dominantFace = i.Key;
                }
            }

            foreach (var i in faceCount) {
                if (i.Value == max && i.Key != dominantFace) {
                    return CubeFace.None;
                }
            }

            return dominantFace;
        }
        
        private void filterMoves(CubeFace f) {
            List<SwipedFace> filteredSwipedFaces = new List<SwipedFace>();

            foreach (var i in swipedFaces) {
                if (i.face == f) {
                    filteredSwipedFaces.Add(i);
                }
            }

            this.swipedFaces = filteredSwipedFaces;
        }
    }
}
