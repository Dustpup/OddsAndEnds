using System;
using System.Collections.Generic;
using System.Threading;
namespace ICE_ObFu.CNinja
{
    partial class StringPointerCarriage
    {
        #region DEBUG TEST
#if DEBUG && CONSOLE
        const bool ISDEBUGING      = false; // Should Skip Over this Script
        const bool WILLSLOWPRO     = false;
        const bool WAIT_FOR_INPUT  = false;
        const bool SKIP_CHAR_INPUT = false;
#endif
        #endregion

        public static string GetIfChunks(string DocumentIn)
        {
            const int _PointerSize = 4;

            string pntrStr           = null;
            string charPointer       = null;
            string linePointer       = null;
            string chunkPointer      = null;

            int st_charPntr = -1;
            int st_thenPntr = -1;
            int st_linePntr = -1;
            int st_chnkPntr = -1;

            int en_charPntr = -1;
            int en_linePntr = -1;
            int en_chnkPntr = -1;


            int ignoreIf    = -1;

            IfStatement foundStat = null;

            List<IfStatement> ifStatements = new List<IfStatement>();

            /*-----------------------------------------------------------------------------------]
             *  Carriage Pointer System Functions Like a pointer  but with 4 char instead of 1
             *                      
             *  pointer =     fun[ctio]n (Test, Test2) 
             *                   ^^^^^^
             *             [Pointer Carraige] -- Grab the 4 characters from the starting char
             *-----------------------------------------------------------------------------------]*/

#if DEBUG && CONSOLE
            if(ISDEBUGING) Console.Clear();
#endif

            for (st_charPntr = 0;st_charPntr < DocumentIn.Length;st_charPntr++) // Chars -- Get Opening statement here
            {
                // Check the end of the carage pointer to see if the carage is at the end of the file.
                
                if      ((st_charPntr + _PointerSize) < DocumentIn.Length)
                     en_charPntr = st_charPntr + _PointerSize;
                else if ((st_charPntr + (_PointerSize - 1)) < DocumentIn.Length)
                     en_charPntr = st_charPntr + (_PointerSize - 1);
                else if ((st_charPntr + (_PointerSize - 2)) < DocumentIn.Length)
                     en_charPntr = st_charPntr + (_PointerSize - 2);
                else if ((st_charPntr + (_PointerSize - 3)) < DocumentIn.Length)
                     en_charPntr = st_charPntr + (_PointerSize - 3);
                else en_charPntr = st_charPntr;

                pntrStr = DocumentIn.Substring(st_charPntr, en_charPntr - st_charPntr);

                if (pntrStr.Length >= 2)
                    charPointer = pntrStr.Substring(0, 2);
                else
                    charPointer = String.Empty;

                #region DEBUG TEST
#if DEBUG && CONSOLE // Character Test
                if (ISDEBUGING)
                {
                    Console.SetCursorPosition(0, 0);
                    Console.CursorVisible = false;
                    string clrdStr = pntrStr.Replace('\n', ' ');
                    clrdStr = clrdStr.Replace('\r', ' ');
                    clrdStr = clrdStr.Replace('\t', ' ');
                    if(WILLSLOWPRO) Thread.Sleep(TimeSpan.FromMilliseconds(1));
                    Console.WriteLine("Point ->:[{0}]", clrdStr);
                    if (WAIT_FOR_INPUT && !SKIP_CHAR_INPUT) Console.ReadKey();
                }
#endif
                #endregion

                if (charPointer == "if" && charPointer != String.Empty)
                {
                    foundStat = new IfStatement();

                    for (st_linePntr = en_charPntr; st_linePntr < DocumentIn.Length; st_linePntr++) // Line
                    {
                        en_linePntr = st_linePntr + _PointerSize;

                        if (DocumentIn.Substring(st_linePntr, en_linePntr - st_linePntr) == "then")
                            st_thenPntr = st_linePntr;

                        if (DocumentIn[st_linePntr] == '\n')
                        {
                            en_linePntr = st_linePntr;
                            linePointer = DocumentIn.Substring(st_charPntr, en_linePntr - st_charPntr);

                            foundStat.IfHeader        = DocumentIn.Substring(st_charPntr, (st_thenPntr + _PointerSize) - st_charPntr);
                            foundStat.StartingPointer = st_charPntr;
                             
                            #region DEBUG TEST
#if DEBUG && CONSOLE // Line Test
                            if (ISDEBUGING)
                            {
                                Console.SetCursorPosition(0, 3);
                                Console.WriteLine(new string(' ', 800));
                                Console.WriteLine(new string(' ', 800));

                                Console.SetCursorPosition(0, 3);
                                Console.WriteLine("Found");
                                Console.WriteLine("Line ->: {0}", linePointer);
                                if (WILLSLOWPRO) Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                                if (WAIT_FOR_INPUT) Console.ReadKey();
                            }
#endif
                            #endregion

                            for (st_chnkPntr = en_linePntr; st_chnkPntr < DocumentIn.Length; st_chnkPntr++) // Chunk -- Get Closing Statement here
                            {

                                if ((st_chnkPntr + _PointerSize) < DocumentIn.Length)
                                    en_chnkPntr = st_chnkPntr + _PointerSize;
                                else if ((st_chnkPntr + (_PointerSize - 1)) < DocumentIn.Length)
                                    en_chnkPntr = st_chnkPntr + (_PointerSize - 1);
                                else if ((st_chnkPntr + (_PointerSize - 2)) < DocumentIn.Length)
                                    en_chnkPntr = st_chnkPntr + (_PointerSize - 2);
                                else if ((st_chnkPntr + (_PointerSize - 3)) < DocumentIn.Length)
                                    en_chnkPntr = st_chnkPntr + (_PointerSize - 3);
                                else en_chnkPntr = st_chnkPntr;

                                pntrStr = DocumentIn.Substring(st_chnkPntr, en_chnkPntr - st_chnkPntr);

                                if (pntrStr.Substring(0,2) == "if") ignoreIf++;

                                if (pntrStr.Substring(0, 3) == "end" && ignoreIf <= 0)
                                {
                                    chunkPointer = DocumentIn.Substring(st_charPntr, en_chnkPntr - st_charPntr);

                                    foundStat.EndingPointer = en_chnkPntr;
                                    foundStat.IfSource      = chunkPointer;
                                    foundStat.IfBody        = DocumentIn.Substring(st_thenPntr,en_chnkPntr - st_thenPntr);
                                    foundStat.ThenPointer   = st_thenPntr;

                                    #region DEBUG TEST
#if DEBUG && CONSOLE // Chunk Test
                                    if (ISDEBUGING)
                                    {
                                        Console.SetCursorPosition(0, 10);
                                        for (int conClr = 0; conClr < 50; conClr++)
                                        {
                                            Console.Write(new string(' ', 800));
                                            Console.WriteLine();
                                        }
                                        Console.SetCursorPosition(0, 10);
                                        Console.WriteLine("Found");
                                        Console.WriteLine("Chunk ->: {0}", chunkPointer);
                                        if (WILLSLOWPRO) Thread.Sleep(TimeSpan.FromMilliseconds(1000));
                                        if (WAIT_FOR_INPUT) Console.ReadKey();
                                    }
#endif
                                    #endregion

                                    break;
                                }
                                else if (pntrStr.Substring(0, 3) == "end" && ignoreIf > 0) ignoreIf--;
                            } 
                            ifStatements.Add(foundStat);
                            break;
                        }
                        
                    }
                }
            }
            return null;

        }
        public static string GetFunctionChunks(string DocumentIn)
        {
            const int _PointerSize = 8;

            int dynPntrSize = -1;
            int ignoreFunc  = 0;

            int st_pntr         = -1;
            int st_linePntr     = -1;
            int st_chunkPntr    = -1;
                                
            int en_pntr         = -1;
            int en_linePntr     = -1;
            int en_chunkpnter   = -1;
                                
            string pnter      = null;
            string linePnter  = null;
            string chunkPnter = null;

            for(st_pntr = 0;st_pntr<DocumentIn.Length;st_pntr++)
            {
                for(dynPntrSize = _PointerSize; dynPntrSize > _PointerSize; dynPntrSize--)
                    if (dynPntrSize <= DocumentIn.Length)
                        break;

                pnter = DocumentIn.Substring(st_pntr, en_pntr - st_pntr);

                if (pnter == "funciton")
                {

                    // starting of function
                    for (st_linePntr = st_pntr; st_linePntr < DocumentIn.Length; st_linePntr++)
                    {
                        if (DocumentIn[st_linePntr] == '\n')
                        {
                            en_linePntr = st_linePntr;
                             // end of header in function
                            for (st_chunkPntr = en_linePntr; st_chunkPntr < DocumentIn.Length; st_chunkPntr++)
                            {
                                en_chunkpnter = st_chunkPntr + 3;
                                // the whole function
                                if (DocumentIn.Substring(st_chunkPntr, en_chunkpnter - st_chunkPntr) == "end")
                                {


                                    break; // TODO: ADD CHECK BEFORE ABRUPTLY ENDING
                                }
                            }
                            break; // TODO: ADD CHECK BEFORE ABRUPTLY ENDING
                        }
                    }
                }
            }


            return null;
        }

    }

    partial class IfStatement
    {
        public string IfHeader {get; set;}
        public string IfBody   {get; set;}
        public string IfSource {get; set;}

        public int StartingPointer = -1;
        public int ThenPointer     = -1;
        public int EndingPointer   = -1;

        List<object> IfElesObjects;

        public IfStatement(string sourceIfStaetment, string ifHeader,string ifBody)
        {
            IfHeader      = ifHeader;
            IfBody        = ifBody;
            IfSource      = sourceIfStaetment;
            IfElesObjects = new List<object>();
        }
        public IfStatement()
        {
            IfHeader      = null;
            IfBody        = null;
            IfSource      = null;
            IfElesObjects = new List<object>();
        }
    }
}
