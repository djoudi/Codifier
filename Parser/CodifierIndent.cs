using Codifier.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codifier.Indent
{
    public class CodifierIndentLinePair
    {
        private int start_line;
        public int StartLine { get { return this.start_line; } }

        private int end_line;
        public int EndLine { get { return this.end_line; } }

        private int start_line_indent;
        public int StartLineIndent { get { return this.start_line_indent; } }

        private int end_line_indent;
        public int EndLineIndent { get { return this.end_line_indent; } }

        public CodifierIndentLinePair(int start_line = 0, int end_line = 0, int start_indent = 0, int end_indent = 0)
        {
            this.start_line = start_line;
            this.end_line = end_line;
            this.start_line_indent = start_indent;
            this.end_line_indent = end_indent;
        }
    }

    public class CodifierIndent
    {
        public const int CODIFIER_INDENT_DEFAULT_INCREMENT_FACTOR = 2;
        private int indent;
        public int Indent
        {
            get { return this.indent; }
            //set { this.indent = value; }
        }

        private int increment_factor;
        public int IncrementFactor { get { return this.increment_factor; } }

        private int current_start_line;
        private int current_end_line;
        private int start_line_indent;

        private string error_message;
        public string ErrorMessage { get { return this.error_message; } set { this.error_message = value; this.error_flag = true; } }
        private bool error_flag;
        public bool ErrorFlag { get { return this.error_flag; } }

        private List<CodifierIndentLinePair> lines_pairs;
        public List<CodifierIndentLinePair> LinesPairs { get { return this.lines_pairs;}}

        private Stack<int> bracket_stack;

        public CodifierIndent(int increment_factor = CODIFIER_INDENT_DEFAULT_INCREMENT_FACTOR)
        {
            this.indent = 0;

            if (increment_factor <= 0)
                this.increment_factor = CODIFIER_INDENT_DEFAULT_INCREMENT_FACTOR;

            this.increment_factor = increment_factor;
            this.current_start_line = -1;
            this.current_end_line = -1;
            this.start_line_indent = -1;
            this.lines_pairs = new List<CodifierIndentLinePair>();
            this.bracket_stack = new Stack<int>();
        }

        public int getTokenIndent(CodifierToken token)
        {
            if (token == null || this.error_flag)
                return -1;

            int temp_indent = this.indent;

            if (token.TokenType == CodifierTokenType.TT_TOKEN_PUNCTUATOR_LEFT_BRACKET)
            {   
                this.indent += this.increment_factor;
                this.current_start_line = token.TokenLineNumber;
                this.start_line_indent = this.indent;

                this.bracket_stack.Push(this.current_start_line);
                this.bracket_stack.Push(this.start_line_indent);

                return temp_indent;
            }
            else if (token.TokenType == CodifierTokenType.TT_TOKEN_PUNCTUATOR_RIGHT_BRACKET)
            {
                if (this.indent == 0)
                    return this.indent;

                this.indent -= this.increment_factor;
                if (this.indent < 0)
                    this.indent = 0;

                this.current_end_line = token.TokenLineNumber;
                
                /*Get start values from stack*/

                if (this.bracket_stack.Count == 0)
                    goto incomplete_block_state;

                this.start_line_indent = this.bracket_stack.Pop();

                if (this.bracket_stack.Count == 0)
                    goto incomplete_block_state;

                this.current_start_line = this.bracket_stack.Pop();

                CodifierIndentLinePair lpair = new CodifierIndentLinePair(
                    this.current_start_line,
                    this.current_end_line,
                    this.start_line_indent,
                    this.indent);
               
                this.lines_pairs.Add(lpair);
                this.current_start_line = this.current_end_line = this.start_line_indent = -1;
            }
            
            return this.indent;

        incomplete_block_state:
            this.ErrorMessage = "Incomplete block (}) missed";
        return -1;
        }

        public void resentIndent()
        {
            this.indent = 0;
            this.current_start_line = this.current_end_line = this.start_line_indent = -1;
            this.lines_pairs.Clear();
            this.bracket_stack.Clear();
        }
    }
}
