using Codifier.AbstractSource;
using Codifier.Error;
using Codifier.Token;
using Codifier.StatesAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codifier.Router
{
	/*
	 * The goal of this class is to detecte the (start state) and route to the appropriate
	 * method to find the target method
	 */

	public class CodifierRouter
	{
		/* All methdos that try to find a specific token take the following signture :
		  public bool tryXXX()
		 * XXX : represents the name of the state that the tokenizer try to find.
		 * return : true if success, otherwise, false.
		 */
		private CodifierAbstractSource abstract_source;
		private bool white_space_as_token_flag;
        public bool WhiteSpaceAsTokenFlag { get { return this.white_space_as_token_flag; } }

		public CodifierRouter(CodifierAbstractSource abstract_source, bool white_space_as_token = true)
		{
			if (abstract_source == null)
				throw new CodifierException(@"Invalid abstract source object");

			this.abstract_source = abstract_source;
			this.white_space_as_token_flag = white_space_as_token;
		}

		public CodifierToken readNextToken()
		{
            int temp_line_number = abstract_source.LineNumber;

			if (this.abstract_source.IsEOS)
				goto eos_state;

			if (this.abstract_source.ErrorFlag)
				goto error_state;

			abstract_source.clearLexemBuffer();

			/* Read token's first character */
			this.abstract_source.readNextCharacter();
			/* Keep token's start index */

            this.abstract_source.LexemeStartPosition = this.abstract_source.CurrentLineCurrentPosition;

			/* White space token*/
			if (char.IsWhiteSpace(this.abstract_source.CurrentCharacter))
			{
				if (this.white_space_as_token_flag)
					return CodifierStatesAPI.whiteSpace(abstract_source);
				else
				{
					this.abstract_source.eatWhiteSpaces();
					abstract_source.clearLexemBuffer();
                    abstract_source.LexemeStartPosition = this.abstract_source.CurrentLineCurrentPosition;
					/*this.abstract_source.readNextCharacter();*/
					if (abstract_source.IsEOS)
						goto eos_state;

					abstract_source.readNextCharacter();
				}
			}

            

			if (this.abstract_source.CurrentCharacter == '_' ||
				Char.IsLetter(this.abstract_source.CurrentCharacter))
				return CodifierStatesAPI.idOrKeyword(abstract_source);
			else if (Char.IsDigit(this.abstract_source.CurrentCharacter))
				return CodifierStatesAPI.number(abstract_source);
			else if (this.abstract_source.CurrentCharacter == ';')
				return new CodifierToken(CodifierTokenType.TT_TOKEN_PUNCTUATOR_SEMI_COLON,
                    this.abstract_source.LexemeBuffer, temp_line_number,
					this.abstract_source.LexemeStartPosition, null);
			else if (this.abstract_source.CurrentCharacter == '\\')
				return CodifierStatesAPI.backSlash(this.abstract_source);
			else if (this.abstract_source.CurrentCharacter == '@')
				return CodifierStatesAPI.at(this.abstract_source);
			else if (this.abstract_source.CurrentCharacter == '\'')
				return CodifierStatesAPI.singleQuote(this.abstract_source);
            else if (this.abstract_source.CurrentCharacter == '\"')
                return CodifierStatesAPI.str(this.abstract_source);
			else if (this.abstract_source.CurrentCharacter == '=')
				return CodifierStatesAPI.equal(abstract_source);
			else if (this.abstract_source.CurrentCharacter == '<')
				return CodifierStatesAPI.lessThan(abstract_source);
			else if (this.abstract_source.CurrentCharacter == '>')
				return CodifierStatesAPI.greaterThan(abstract_source);
			else if (this.abstract_source.CurrentCharacter == '?')
				return new CodifierToken(CodifierTokenType.TT_TOKEN_OPERATOR_QUESTION_MARK,
                    abstract_source.LexemeBuffer, temp_line_number,
				this.abstract_source.LexemeStartPosition, null);
			else if (this.abstract_source.CurrentCharacter == '+')
				return CodifierStatesAPI.plus(abstract_source);
			else if (this.abstract_source.CurrentCharacter == '-')
				return CodifierStatesAPI.minus(abstract_source);
			else if (this.abstract_source.CurrentCharacter == '&')
				return CodifierStatesAPI.and(abstract_source);
			else if (this.abstract_source.CurrentCharacter == '|')
				return CodifierStatesAPI.or(abstract_source);
			else if (this.abstract_source.CurrentCharacter == '!')
				return CodifierStatesAPI.not(abstract_source);
			else if (this.abstract_source.CurrentCharacter == '*')
				return CodifierStatesAPI.multiply(abstract_source);
			else if (this.abstract_source.CurrentCharacter == '/')
				return CodifierStatesAPI.divide(abstract_source);
			else if (this.abstract_source.CurrentCharacter == '%')
				return CodifierStatesAPI.percent(abstract_source);
			else if (this.abstract_source.CurrentCharacter == '^')
				return CodifierStatesAPI.caret(abstract_source);
            else if (this.abstract_source.CurrentCharacter == '.')
                return CodifierStatesAPI.dot(abstract_source);
            else if (this.abstract_source.CurrentCharacter == ',')
                return new CodifierToken(CodifierTokenType.TT_TOKEN_PUNCTUATOR_COMMA, abstract_source.LexemeBuffer,
                    this.abstract_source.LineNumber, this.abstract_source.LexemeStartPosition, null);
            else if (this.abstract_source.CurrentCharacter == ':')
                return new CodifierToken(CodifierTokenType.TT_TOKEN_PUNCTUATOR_COLON, abstract_source.LexemeBuffer,
                    temp_line_number, this.abstract_source.LexemeStartPosition, null);
            else if (this.abstract_source.CurrentCharacter == '{')
                return new CodifierToken(CodifierTokenType.TT_TOKEN_PUNCTUATOR_LEFT_BRACKET, abstract_source.LexemeBuffer,
                   temp_line_number, this.abstract_source.LexemeStartPosition, null);
            else if (this.abstract_source.CurrentCharacter == '}')
                return new CodifierToken(CodifierTokenType.TT_TOKEN_PUNCTUATOR_RIGHT_BRACKET, abstract_source.LexemeBuffer,
                    temp_line_number, this.abstract_source.LexemeStartPosition, null);
            else if (this.abstract_source.CurrentCharacter == '[')
                return new CodifierToken(CodifierTokenType.TT_TOKEN_PUNCTUATOR_LEFT_SQUARE_BRACKET, abstract_source.LexemeBuffer,
                    temp_line_number, this.abstract_source.LexemeStartPosition, null);
            else if (this.abstract_source.CurrentCharacter == ']')
                return new CodifierToken(CodifierTokenType.TT_TOKEN_PUNCTUATOR_RIGHT_SQUARE_BRACKET, abstract_source.LexemeBuffer,
                    temp_line_number, this.abstract_source.LexemeStartPosition, null);
            else if (this.abstract_source.CurrentCharacter == '(')
                return new CodifierToken(CodifierTokenType.TT_TOKEN_PUNCTUATOR_LEFT_PARNETHESES, abstract_source.LexemeBuffer,
                    temp_line_number, this.abstract_source.LexemeStartPosition, null);
            else if (this.abstract_source.CurrentCharacter == ')')
                return new CodifierToken(CodifierTokenType.TT_TOKEN_PUNCTUATOR_RIGHT_PARNETHESES, abstract_source.LexemeBuffer,
                    temp_line_number, this.abstract_source.LexemeStartPosition, null);
            else
            {
                this.abstract_source.ErrorMessage = @"Uknown character";
                goto error_state;
            }

		eos_state:
            return new CodifierToken(CodifierTokenType.TT_TOKEN_EOS, null, temp_line_number,
				this.abstract_source.LexemeStartPosition, null);
    error_state:
        return CodifierStatesAPI.error(abstract_source);
		}
	}
}
