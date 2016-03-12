using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticsLab
{
    enum directions {LEFT, TOP, DIAGONAL};
    class PairWiseAlign
    {
        int MaxCharactersToAlign;

        public PairWiseAlign()
        {
            // Default is to align only 5000 characters in each sequence.
            this.MaxCharactersToAlign = 5000;
        }

        public int getMaxhCharactersToAlign()
        {
            return this.MaxCharactersToAlign;
        }
        public PairWiseAlign(int len)
        {
            // Alternatively, we can use an different length; typically used with the banded option checked.
            this.MaxCharactersToAlign = len;
        }

        /// <summary>
        /// this is the function you implement.
        /// </summary>
        /// <param name="sequenceA">the first sequence</param>
        /// <param name="sequenceB">the second sequence, may have length not equal to the length of the first seq.</param>
        /// <param name="banded">true if alignment should be band limited.</param>
        /// <returns>the alignment score and the alignment (in a Result object) for sequenceA and sequenceB.  The calling function places the result in the dispay appropriately.
        /// 
        public ResultTable.Result Align_And_Extract(GeneSequence sequenceA, GeneSequence sequenceB, bool banded)
        {
            ResultTable.Result result = new ResultTable.Result();
            int score;                                                       // place your computed alignment score here
            string[] alignment = new string[2];                              // place your two computed alignments here


            // ********* these are placeholder assignments that you'll replace with your code  *******
            score = 0;                                                
            alignment[0] = "";
            alignment[1] = "";
            // ***************************************************************************************

            // Limiting the lengths of the sequences to the max characters to align
            int lengthOfSequenceA = Math.Min(sequenceA.Sequence.Length, MaxCharactersToAlign);
            int lengthOfSequenceB = Math.Min(sequenceB.Sequence.Length, MaxCharactersToAlign);

            // Create two arrays to hold the intermediate values and the alignment details
            int[,] values = new int[lengthOfSequenceA + 1, lengthOfSequenceB + 1];
            directions[,] prev = new directions[lengthOfSequenceA + 1, lengthOfSequenceB + 1];
            
            // first fill first row and column with cost of inserts/deletes
            for (int column = 0; column < lengthOfSequenceB + 1; column++)
            {
                values[0, column] = column * 5;
                prev[0, column] = directions.LEFT;
            }
            for (int row = 0; row < lengthOfSequenceA + 1; row++)
            {
                values[row, 0] = row * 5;
                prev[row, 0] = directions.TOP;
            }

            // Now iterate through the rest of the cells filling out the min value for each
            for (int row = 1; row < lengthOfSequenceA + 1; row++)
            {
                for (int column = 1; column < lengthOfSequenceB + 1; column++)
                {
                    // Compute values for each direction
                    int costOfTop_Delete = values[row-1, column] + 5;
                    int costOfLeft_Insert = values[row, column-1] + 5;
                    // Compute cost of moving from diagonal depending on whether the letters match
                    int costOfMovingFromDiagonal = (sequenceA.Sequence[row-1] == sequenceB.Sequence[column-1])? -3 : 1;
                    int costOfDiagonal = values[row - 1, column - 1] + costOfMovingFromDiagonal;

                    // value of cell would be the minimum cost out of the three directions
                    int costOfMin = Math.Min(costOfTop_Delete, Math.Min(costOfLeft_Insert, costOfDiagonal));
                    values[row, column] = costOfMin;
                    
                    // Store the direction
                    if (costOfMin == costOfTop_Delete)
                    {
                        prev[row, column] = directions.TOP;
                    } 
                    else if (costOfMin == costOfLeft_Insert)
                    {
                        prev[row, column] = directions.LEFT;
                    }
                    else
                    {
                        prev[row, column] = directions.DIAGONAL;
                    }
                }
            }

            // score would be value of the last cell
            score = values[lengthOfSequenceA, lengthOfSequenceB];

            // Create the alignments
            int rowIterator = lengthOfSequenceA, columnIterator = lengthOfSequenceB;
            StringBuilder first = new StringBuilder(), second = new StringBuilder();
            while (rowIterator != 0 && columnIterator != 0)
            {
                //Console.WriteLine("row is " + rowIterator);
                //Console.WriteLine("column is " + columnIterator);
                if (prev[rowIterator, columnIterator] == directions.DIAGONAL) // match/sub
                {
                    first.Insert(0, sequenceA.Sequence[rowIterator-1]);
                    second.Insert(0, sequenceB.Sequence[columnIterator-1]);
                } 
                else if (prev[rowIterator, columnIterator] == directions.TOP) //delete
                {
                    first.Insert(0, sequenceA.Sequence[rowIterator-1]);
                    second.Insert(0, '-');
                }
                else // insert
                {
                    first.Insert(0, '-');
                    second.Insert(0, sequenceB.Sequence[columnIterator-1]);
                }
                rowIterator--;
                columnIterator--;
            }
            alignment[0] = first.ToString();
            alignment[1] = second.ToString();

            result.Update(score,alignment[0],alignment[1]);                  // bundling your results into the right object type 
            return(result);
        }
    }
}
