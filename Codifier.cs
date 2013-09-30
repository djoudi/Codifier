using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Codifier.AbstractSource;
using Codifier.Router;
using Codifier.Error;
using Codifier.Token;

namespace Codifier{

    public enum CodifierSourceType
    {
        CODIFIER_SOURCE_STRING,
        CODIFIER_SOURCE_FILE
    }

    public class Codifier
    {
        private CodifierAbstractSource abstract_source;
        public CodifierAbstractSource AbstractSource { get { return this.abstract_source; } }
        
        private CodifierRouter router;
        public CodifierRouter Router { get { return this.router; } }

        private CodifierAbstractSourceType source_type;
        public CodifierAbstractSourceType SourceType { get { return this.source_type; } }

        private bool is_eos;
        public bool isEOS { get { return this.is_eos; }}
        public const int CODIFIER_TOKENS_HISTORY_MAX_SIZE = 8;

        public Codifier(string source, CodifierAbstractSourceType source_type,bool white_space_as_token = true,
            int tokens_history_max_size = CODIFIER_TOKENS_HISTORY_MAX_SIZE)
        {
            if (source_type == CodifierAbstractSourceType.T_ABSTRACT_SOURCE_TYPE_STRING)
                this.abstract_source = new CodifierAbstractSource(source, tokens_history_max_size);
            else
                this.abstract_source = CodifierAbstractSource.SourceFromFile(source, tokens_history_max_size);

            this.source_type = source_type;

            this.router = new CodifierRouter(this.abstract_source,white_space_as_token);
            this.is_eos = false;
        }

        public CodifierToken nextToken()
        {
            CodifierToken token = this.router.readNextToken();
            if (token.TokenType == CodifierTokenType.TT_TOKEN_EOS)
                this.is_eos = true;

            return token;
        }

        public void resetTokenizer()
        {
            this.abstract_source.resetSource();
            this.is_eos = false;
        }
    }

   
}
