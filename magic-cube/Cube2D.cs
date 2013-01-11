using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace magic_cube {
    class Cube2D {
        private int size;
        private CubeFace[,] projection;
        
        public Cube2D(int size) {
            this.size = size;
            this.projection = new CubeFace[size*4,size*3];
            createCube();
        }

        private void createCube(){
            for(int i=0; i<size*4; i++){
                for(int j=0; j<size*3; j++){
                    if (i < size && j >= size && j < size * 2) {
                        projection[i, j] = CubeFace.B;
                    }
                    else if (i >= size && i < size * 2) {
                        if (j < size) {
                            projection[i, j] = CubeFace.L;
                        }
                        else if (j >= size && j < size * 2) {
                            projection[i, j] = CubeFace.D;
                        }
                        else {
                            projection[i, j] = CubeFace.R;
                        }
                    }
                    else if (i >= size * 2 && i < size * 3 && j >= size && j < size * 2) {
                        projection[i, j] = CubeFace.F;
                    }
                    else if (i >= size * 3 && j >= size && j < size * 2) {
                        projection[i, j] = CubeFace.U;
                    }
                    else {
                        projection[i, j] = CubeFace.None;
                    }
                }
            }
        }

        public void rotate(KeyValuePair<Move, RotationDirection> move) {
            switch (move.Key) {
                case Move.F:
                    break;
                case Move.B:
                    break;
                case Move.R:
                    break;
                case Move.L:
                case Move.M:
                    rotateLM(move.Key, move.Value);
                    break;
                case Move.U:
                    break;
                case Move.D:
                    break;
                case Move.E:
                    break;
                case Move.S:
                    break;
            }
        }

        private void rotateLM(Move m, RotationDirection d) {
            CubeFace t;
            int j;

            List<List<int>> substitutions = new List<List<int>> {
                new List<int>{5, 0, 3, 0},
                new List<int>{5, 2, 5, 0},
                new List<int>{3, 2, 5, 2},
                new List<int>{3, 0, 3, 2},
                new List<int>{4, 2, 5, 1},
                new List<int>{3, 1, 4, 2},
                new List<int>{4, 0, 3, 1},
                new List<int>{5, 1, 4, 0},
            };

            if (m == Move.L) {
                j = 3;
            }
            else {
                j = 4;
            }

            if(d == RotationDirection.ClockWise){
                t = projection[9, j];
                projection[9, j] = projection[0, j];
                projection[0, j] = projection[3, j];
                projection[3, j] = projection[6, j];
                projection[6, j] = t;

                t = projection[4, j];
                projection[4, j] = projection[7, j];
                projection[7, j] = projection[10, j];
                projection[10, j] = projection[1, j];
                projection[1, j] = t;

                t = projection[11, j];
                projection[11, j] = projection[2, j];
                projection[2, j] = projection[5, j];
                projection[5, j] = projection[8, j];
                projection[8, j] = t;

            }
            else{
                t = projection[0, j];
                projection[0, j] = projection[9, j];
                projection[9, j] = projection[6, j];
                projection[6, j] = projection[3, j];
                projection[3, j] = t;

                t = projection[1, j];
                projection[1, j] = projection[10, j];
                projection[10, j] = projection[7, j];
                projection[7, j] = projection[4, j];
                projection[4, j] = t;

                t = projection[2, j];
                projection[2, j] = projection[11, j];
                projection[11, j] = projection[8, j];
                projection[8, j] = projection[5, j];
                projection[5, j] = t;
            }

            if (m == Move.L) {
                int first_lhs = 0, second_lhs = 1, first_rhs = 2, second_rhs = 3;

                if (d == RotationDirection.CounterClockWise) {
                    first_lhs = 2;
                    second_lhs = 3;

                    first_rhs = 0;
                    second_rhs = 1;
                }

                t = projection[substitutions[0][first_lhs], substitutions[0][second_lhs]];

                for (int i = 0; i < substitutions.Count - 1; i++) {
                    projection[substitutions[i][first_lhs], substitutions[i][second_lhs]] = projection[substitutions[i][first_rhs], substitutions[i][second_rhs]];
                }

                projection[substitutions[substitutions.Count - 1][first_lhs], substitutions[substitutions.Count - 1][second_lhs]] =
                    projection[substitutions[substitutions.Count - 1][first_rhs], substitutions[substitutions.Count - 1][second_rhs]];
            }
        }

        public void dbg(){
            for (int i = 0; i < size * 4; i++) {
                for (int j = 0; j < size * 3; j++) {
                    Debug.Write(projection[i, j].ToString().PadLeft(5, ' '));
                }
                Debug.WriteLine("");
            }
        }
    }
}
