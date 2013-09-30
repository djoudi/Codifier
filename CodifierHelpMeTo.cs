using Codifier.AbstractSource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codifier.KeywordsDictionary;
using Codifier.Values;
namespace Codifier.Helpers
{
    /* The goal of this class is to work as a helper by provide a general REPEATED sub-states to help other states
     * and to avoid the repetition 
     */
    public class CodifierHelpMeTo
    {
        public static bool readUnicodeCharacter(CodifierAbstractSource abstract_source)
        {
            int hexadecimal_characters_count = 1;
            char u_or_U = '\0';

            if (abstract_source == null)
                return false;

            /*if (abstract_source == null || (abstract_source.CurrentCharacter != 'u' &&
               abstract_source.CurrentCharacter != 'U'))
                return false;*/

            hexadecimal_characters_count = ((abstract_source.CurrentCharacter == 'u') ? 4 : 8);

            if (abstract_source.IsEOS) // the value is : \u | \U ONLY
            {
                abstract_source.ErrorMessage =
                    string.Format("{0} hexadeciaml-digits missed after the unicode escape character",
                    hexadecimal_characters_count);
                return false;
            }

            u_or_U = abstract_source.CurrentCharacter;

            for (int i = 1; i <= hexadecimal_characters_count; i++)
            {
                abstract_source.readNextCharacter();
                if ((!CodifierValues.HexadecimalValues.Contains(abstract_source.CurrentCharacter.ToString()))
                    || (abstract_source.IsEOS && (i < hexadecimal_characters_count)))
                {
                    if (!abstract_source.IsEOS)
                        abstract_source.readPreviousCharacter();

                    abstract_source.ErrorMessage = "Invalid unicode character";
                    return false;
                }
            } 

            return true;

            ////////////////////////////
        }

        // \x hex-digit hex-digit(op) hex-digit(op) hex-digit(op)
        public static bool readHexadecimalEscapeCharacter(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return false;

            if (abstract_source.IsEOS) // \x only
            {
                abstract_source.ErrorMessage = "incompleted hexadecimal escape character";
                return false;
            }

            int hexadecimal_characters_count = 1;

            abstract_source.readNextCharacter();
            while (CodifierValues.HexadecimalValues.Contains(abstract_source.CurrentCharacter.ToString()))
            {
                if (hexadecimal_characters_count < 4)
                    hexadecimal_characters_count++;
                else
                    break;

                abstract_source.readNextCharacter();
            }

            if (hexadecimal_characters_count == 1)
            {
                abstract_source.ErrorMessage = "Invalide hexadecimal escape character";
                return false;
            }

            return true;
        }
        
        /* Check for escape characters \' \" \\ \0 \a \b \f \n \r \t \v \x */
        public static bool readEscapeCharacter(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null || abstract_source.IsEOS)
                return false;

            abstract_source.readNextCharacter();

            if (CodifierValues.EscapeCharacterValues.Contains(abstract_source.CurrentCharacter.ToString()))
            {
                /* Do replacement */
               // abstract_source.readPreviousCharacter();
                //abstract_source.addToBuffer("\\");
                abstract_source.readNextCharacter();
                return true;
            }
            else
            {
                abstract_source.ErrorMessage = "Invalide escape character";
                return false;
            }
        }
        
        /* This helper used to read one character for character literal or string literal */
        public static bool readCharacter(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return false;

           /* if (abstract_source.IsEOS)
                return true;*/

            abstract_source.readNextCharacter();
            if (abstract_source.CurrentCharacter == '\\')
            {
                if (abstract_source.IsEOS)
                {
                    abstract_source.ErrorMessage = "Invalide escape character";
                    return false;
                }

                abstract_source.readNextCharacter();
                if (abstract_source.CurrentCharacter == 'X')
                {
                    abstract_source.ErrorMessage = "x capital not allowed for hexadecimal escape character";
                    return false;
                }
                else if (abstract_source.CurrentCharacter == 'x')
                    return CodifierHelpMeTo.readHexadecimalEscapeCharacter(abstract_source);
                else if (Char.ToUpper(abstract_source.CurrentCharacter) == 'U')
                    return CodifierHelpMeTo.readUnicodeCharacter(abstract_source);
                else
                {
                    abstract_source.readPreviousCharacter();
                    return CodifierHelpMeTo.readEscapeCharacter(abstract_source);
                }
            }

            return true;
        }

        /* read regular or verbatim string */
        public static bool readString(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return false;

            bool verbatim_string_flag = abstract_source.LexemeBuffer.StartsWith("@");

            if (abstract_source.IsEOS)
            {
                if(verbatim_string_flag)
                    abstract_source.ErrorMessage = "Incomplete verbatim string";
                else
                    abstract_source.ErrorMessage = "Incomplete string";
                return false;
            }

            /* start by verbatim string */
            if (verbatim_string_flag)
            {
                while (abstract_source.readNextCharacter())
                {
                    if (abstract_source.CurrentCharacter == '\"' && abstract_source.IsEOS)
                        return true;
                    else if (abstract_source.CurrentCharacter == '\"')
                    {
                        /* check for ""value..." pattern */
                        abstract_source.readNextCharacter();
                        if (abstract_source.CurrentCharacter != '\"')
                        {
                            abstract_source.readPreviousCharacter();
                            return true;
                        }
                    }
                }

                abstract_source.ErrorMessage = "Incomplete verbatim string";
                return false;

            }

            /* regular string */

            while (true) 
            {
                if (!CodifierHelpMeTo.readCharacter(abstract_source))
                    return false;

                if (abstract_source.CurrentCharacter == '\"')
                    return true;
                else if (abstract_source.CurrentCharacter == '\n')
                {
                    abstract_source.ErrorMessage = "New line character is not allowed in regular string";
                    return false;
                }
                else if (abstract_source.IsEOS)
                {
                    abstract_source.ErrorMessage = "Infinite string";
                    return false;
                }
            }
        }

        /* [0-9] */
        public static bool readDecimalCharacter(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return false;

            if (abstract_source.IsEOS) // one digit
                return true;

          //  abstract_source.readNextCharacter();

            while (abstract_source.readNextCharacter())
            {
                if (!Char.IsDigit(abstract_source.CurrentCharacter))
                    break;
            }

            if(!Char.IsDigit(abstract_source.CurrentCharacter))
                abstract_source.readPreviousCharacter();

            return true;
        }

        /* [0-9a-fA-F] */
        public static bool readHexadecimalCharacter(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return false;

            if (abstract_source.IsEOS) // one digit
            {
                abstract_source.ErrorMessage = "Invalid hexadecimal value";
                return false;
            }

            abstract_source.readNextCharacter();

            if (!CodifierValues.HexadecimalValues.Contains(abstract_source.CurrentCharacter.ToString())) // one digit
            {
                abstract_source.ErrorMessage = "Invalid hexadecimal value";
                return false;
            }

            if (abstract_source.IsEOS)
                return true;

            while (abstract_source.readNextCharacter())
            {
                if (!CodifierValues.HexadecimalValues.Contains(abstract_source.CurrentCharacter.ToString()))
                    break;
            }

            if (!Char.IsDigit(abstract_source.CurrentCharacter))
                abstract_source.readPreviousCharacter();

            return true;
        }

         /* U L u l */
        public static bool readIntegerTypeSuffixCharacter(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return false;

            if (abstract_source.IsEOS) // one digit
                return true;

            char first_suffix_value;
            abstract_source.readNextCharacter();
            if (Char.ToUpper(abstract_source.CurrentCharacter) != 'L' &&
                Char.ToUpper(abstract_source.CurrentCharacter) != 'U')
            {
                abstract_source.readPreviousCharacter();
                return true;
            }

            first_suffix_value = abstract_source.CurrentCharacter; // hold it to avoid repeatition LL UU

            if (abstract_source.IsEOS)
                return true;

            abstract_source.readNextCharacter();
            if (Char.ToUpper(abstract_source.CurrentCharacter) != 'L' &&
                Char.ToUpper(abstract_source.CurrentCharacter) != 'U')
            {
                abstract_source.readPreviousCharacter();
                return true;
            }

            if (Char.ToUpper(first_suffix_value) != Char.ToUpper(abstract_source.CurrentCharacter))
                return true;

            abstract_source.readPreviousCharacter();
            return true;
        }

        /* F f D d M m */
        public static bool readRealTypeSuffixCharacter(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return false;

            if (abstract_source.IsEOS) // one digit
                return true;

            
            abstract_source.readNextCharacter();
            if (!CodifierValues.RealTypeSuffixValues.Contains(abstract_source.CurrentCharacter.ToString()))
            {
                abstract_source.readPreviousCharacter();
                return true;
            }

            return true;
        }

        /* (e|E){1} (+|-)? decimal-dgits */

        public static bool readRealExponentPart(CodifierAbstractSource abstract_source)
        {
            if (abstract_source == null)
                return false;

            if (abstract_source.IsEOS) // one digit
                goto error_state;

            abstract_source.readNextCharacter();
            if(Char.IsDigit(abstract_source.CurrentCharacter))
                return CodifierHelpMeTo.readDecimalCharacter(abstract_source);
            else if(abstract_source.CurrentCharacter != '+' &&
                    abstract_source.CurrentCharacter != '-' )
                goto error_state;

            if (abstract_source.IsEOS)
                goto error_state;

            abstract_source.readNextCharacter();
            if (!Char.IsDigit(abstract_source.CurrentCharacter))
                goto error_state;

            return CodifierHelpMeTo.readDecimalCharacter(abstract_source);

            error_state :
             abstract_source.ErrorMessage = "Invalid exponent value";
                return false;
        }
    }
}
