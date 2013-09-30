using Codifier.Error;
using Codifier.Token;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codifier.AbstractSource
{
    public enum CodifierAbstractSourceType
    {
        T_ABSTRACT_SOURCE_TYPE_STRING,
        T_ABSTRACT_SOURCE_TYPE_FILE
    }

    public class CodifierAbstractSource
    {
        public const int CODIFIER_TOKENS_HISTORY_MAX_SIZE = 8;
        private CodifierAbstractSourceType tokenizer_source_type;

        private string file_path; /* null if tokenizer_source_type = T_ABSTRACT_SOURCE_TYPE_STRING */
        public string FilePath { get { return this.file_path; } }

        private string source_code;
        public string SourceCode { get { return this.source_code; } }

        private int source_code_length; // in byte
        public int SourceCodeLength { get { return this.source_code_length; } }

        private int source_code_current_position;
        public int SourceCodeCurrentPosition
        {
            get { return this.source_code_current_position; }
            set { this.source_code_current_position = value; }
        }

        private int lexeme_start_position; /* keep the value that represent the first character index the current token being read */
        public int LexemeStartPosition
        {
            get { return this.lexeme_start_position; }
            set { this.lexeme_start_position = value; }
        }

        private int current_line_current_position;
        public int CurrentLineCurrentPosition { get { return this.current_line_current_position; } }

        private string lexeme_buffer;
        public string LexemeBuffer { get { return this.lexeme_buffer; } }

        private int lexeme_buffer_current_position;
        public int LexemeBufferCurrentPosition { get { return this.lexeme_buffer_current_position; } }

        private char current_character;
        public char CurrentCharacter
        {
            get
            {
                return this.current_character;
            }

            set
            {
                this.current_character = value;
            }
        }

        private bool is_eos; /* EOS : End Of Source */
        public bool IsEOS
        {
            get
            {
                return (this.is_eos = (this.source_code_current_position >= this.source_code_length));
            }
        }

        private int line_number;
        public int LineNumber { get { return this.line_number; }}

        private List<CodifierToken> tokens_history;
        public List<CodifierToken> TokensHistory { get { return this.tokens_history; } }

        private int tokens_history_max_size; /* default size : 8, if the size = -1, the history will keep all tokens, 0 for nothing */
        public int TokensHistoryMaxSize { get { return this.tokens_history_max_size; } }

        /* private bool return_white_space_as_token_falg;*/
        

        private string error_message;
       
        public string ErrorMessage
        {
            get
            {
                return this.error_message;
            }
            set
            {
                this.error_message = value; this.error_flag = true;
            }
        }

        private bool error_flag;
        public bool ErrorFlag
        {
            get
            {
                return (this.error_flag = (this.error_message != null));
            }
        }

        //private Token last_detected_token;

        public CodifierAbstractSource(string source_code, int tokens_history_max_size = CODIFIER_TOKENS_HISTORY_MAX_SIZE)
        {
            this.tokenizer_source_type = CodifierAbstractSourceType.T_ABSTRACT_SOURCE_TYPE_STRING;
            this.initializeAbstractSource(source_code, tokens_history_max_size);
        }

        public static CodifierAbstractSource SourceFromFile(string file_path, int tokens_history_max_size = CODIFIER_TOKENS_HISTORY_MAX_SIZE)
        {
            CodifierAbstractSource abstract_source = null;

            if (string.IsNullOrEmpty(file_path))
                throw new CodifierException(string.Format("Invalid file path : {0}", file_path));

            string source_code = null;

            try
            {
                source_code = File.ReadAllText(file_path);
            }
            catch (Exception e)
            {
                throw new CodifierException(e.Message);
            }

            abstract_source = new CodifierAbstractSource(source_code);
            abstract_source.tokenizer_source_type = CodifierAbstractSourceType.T_ABSTRACT_SOURCE_TYPE_FILE;
            abstract_source.initializeAbstractSource(source_code, tokens_history_max_size); // initialize file_path to null
            abstract_source.file_path = file_path;

            return abstract_source;

        }

        public void resetSource()
        {
         
            this.source_code_current_position = 0;

            this.lexeme_start_position = 0;

            this.current_line_current_position = 0;


            this.current_character = '\0';

            this.is_eos = false;

            this.line_number = 1;

            this.error_flag = false;
            this.error_message = null;

            this.clearLexemBuffer();
        }

        protected void initializeAbstractSource(string source_code, int tokens_history_max_size = CODIFIER_TOKENS_HISTORY_MAX_SIZE)
        {
            if (string.IsNullOrEmpty(source_code))
                this.source_code = "";//throw new CodifierException("There is no code to analyze !");

            this.file_path = null;

            this.source_code = source_code;
            this.source_code_length = this.source_code.Length;

            if (tokens_history_max_size <= 0)
                this.tokens_history = null;
            else
                this.tokens_history = new List<CodifierToken>();

            this.tokens_history_max_size = tokens_history_max_size;

            this.resetSource();
        }

        public void clearLexemBuffer()
        {
            this.lexeme_buffer = "";
            this.lexeme_buffer_current_position = 0;
        }

        public bool readNextCharacter()
        {
            if (this.IsEOS || this.ErrorFlag)
            {
                return false;
            }

            this.current_character = this.source_code[this.source_code_current_position++];
            this.current_line_current_position++;

            this.lexeme_buffer += this.current_character;
            this.lexeme_buffer_current_position++;

            if (this.current_character == '\n')
            {
                this.line_number++;
                this.current_line_current_position = 0;
            }

            return true;
        }

        public bool readPreviousCharacter()
        {
            if (this.ErrorFlag)
                return false;
            if (this.source_code_current_position == 0)
            {
                return false;
            }

            this.current_character = this.source_code[--this.source_code_current_position];
            this.current_line_current_position--;

            if (this.current_character == '\n')
               this.line_number--;
            

            if (this.lexeme_buffer_current_position > 0)
            {
                if ((this.lexeme_buffer.Length - 1) > 0)
                {
                    this.lexeme_buffer = this.lexeme_buffer.Substring(0, this.lexeme_buffer.Length - 1);
                }
            }
            return true;
        }

        public void eatWhiteSpaces()
        {
            while (this.readNextCharacter())
            {
                if (!Char.IsWhiteSpace(this.current_character))
                {
                    this.readPreviousCharacter();
                    break;
                }
            }
        }

        public bool bufferGoBackOneCharacter()
        {
            if (this.lexeme_buffer_current_position == 0)
                return false;

            this.lexeme_buffer_current_position--;
            if ((this.lexeme_buffer.Length - 1) > 0)
                this.lexeme_buffer = this.lexeme_buffer.Substring(0, this.lexeme_buffer.Length - 1);
            return true;
        }

        public void addToBuffer(string extra_val)
        {
            if (string.IsNullOrEmpty(extra_val))
                return;

            this.lexeme_buffer += extra_val;
            this.lexeme_buffer_current_position += extra_val.Length;
        }

        //public string getTokenizerErrorMessage() { }
    }
}
