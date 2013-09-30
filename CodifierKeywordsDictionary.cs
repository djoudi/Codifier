using Codifier.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codifier.KeywordsDictionary
{
    public static partial class CodifierKeywordsDictionary
    {

        /* C# Keywords */
        public static Dictionary<string, CodifierTokenType> KeywordsDictionary = new Dictionary<string, CodifierTokenType>() {
 
         {"abstract"       ,CodifierTokenType.TT_TOKEN_KEYWORD_ABSTRACT    },
         {"as"             ,CodifierTokenType.TT_TOKEN_KEYWORD_AS          },
         {"base"           ,CodifierTokenType.TT_TOKEN_KEYWORD_BASE        },
         {"bool"           ,CodifierTokenType.TT_TOKEN_KEYWORD_BOOL        },
         {"break"          ,CodifierTokenType.TT_TOKEN_KEYWORD_BREAK       },
         {"byte"           ,CodifierTokenType.TT_TOKEN_KEYWORD_BYTE        },
         {"case"           ,CodifierTokenType.TT_TOKEN_KEYWORD_CASE        },
         {"catch"          ,CodifierTokenType.TT_TOKEN_KEYWORD_CATCH       },
         {"char"           ,CodifierTokenType.TT_TOKEN_KEYWORD_CHAR        },
         {"checked"        ,CodifierTokenType.TT_TOKEN_KEYWORD_CHECKED     },
         {"class"          ,CodifierTokenType.TT_TOKEN_KEYWORD_CLASS       },
         {"const"          ,CodifierTokenType.TT_TOKEN_KEYWORD_CONST       },
         {"continue"       ,CodifierTokenType.TT_TOKEN_KEYWORD_CONTINUE    },
         {"decimal"        ,CodifierTokenType.TT_TOKEN_KEYWORD_DECIMAL     },
         {"default"        ,CodifierTokenType.TT_TOKEN_KEYWORD_DEFAULT     },
         {"delegate"       ,CodifierTokenType.TT_TOKEN_KEYWORD_DELEGATE    },
         {"do"             ,CodifierTokenType.TT_TOKEN_KEYWORD_DO          },
         {"double"         ,CodifierTokenType.TT_TOKEN_KEYWORD_DOUBLE      },
         {"else"           ,CodifierTokenType.TT_TOKEN_KEYWORD_ELSE        },
         {"enum"           ,CodifierTokenType.TT_TOKEN_KEYWORD_ENUM        },
         {"event"          ,CodifierTokenType.TT_TOKEN_KEYWORD_EVENT       },
         {"explicit"       ,CodifierTokenType.TT_TOKEN_KEYWORD_EXPLICIT    },
         {"extern"         ,CodifierTokenType.TT_TOKEN_KEYWORD_EXTERN      },
         {"false"          ,CodifierTokenType.TT_TOKEN_KEYWORD_FALSE       },
         {"finally"        ,CodifierTokenType.TT_TOKEN_KEYWORD_FINALLY     },
         {"fixed"          ,CodifierTokenType.TT_TOKEN_KEYWORD_FIXED       },
         {"float"          ,CodifierTokenType.TT_TOKEN_KEYWORD_FLOAT       },
         {"for"            ,CodifierTokenType.TT_TOKEN_KEYWORD_FOR         },
         {"foreach"        ,CodifierTokenType.TT_TOKEN_KEYWORD_FOREACH     },
         {"goto"           ,CodifierTokenType.TT_TOKEN_KEYWORD_GOTO        },
         {"if"             ,CodifierTokenType.TT_TOKEN_KEYWORD_IF          },
         {"implicit"       ,CodifierTokenType.TT_TOKEN_KEYWORD_IMPLICIT    },
         {"in"             ,CodifierTokenType.TT_TOKEN_KEYWORD_IN          },
         {"int"            ,CodifierTokenType.TT_TOKEN_KEYWORD_INT         },
         {"interface"      ,CodifierTokenType.TT_TOKEN_KEYWORD_INTERFACE   },
         {"internal"       ,CodifierTokenType.TT_TOKEN_KEYWORD_INTERNAL    },
         {"is"             ,CodifierTokenType.TT_TOKEN_KEYWORD_IS          },
         {"lock"           ,CodifierTokenType.TT_TOKEN_KEYWORD_LOCK        },
         {"long"           ,CodifierTokenType.TT_TOKEN_KEYWORD_LONG        },
         {"namespace"      ,CodifierTokenType.TT_TOKEN_KEYWORD_NAMESPACE   },
         {"new"            ,CodifierTokenType.TT_TOKEN_KEYWORD_NEW         },
         {"null"           ,CodifierTokenType.TT_TOKEN_KEYWORD_NULL        },
         {"object"         ,CodifierTokenType.TT_TOKEN_KEYWORD_OBJECT      },
         {"operator"       ,CodifierTokenType.TT_TOKEN_KEYWORD_OPERATOR    },
         {"out"            ,CodifierTokenType.TT_TOKEN_KEYWORD_OUT         },
         {"override"       ,CodifierTokenType.TT_TOKEN_KEYWORD_OVERRIDE    },
         {"params"         ,CodifierTokenType.TT_TOKEN_KEYWORD_PARAMS      },
         {"partial"        ,CodifierTokenType.TT_TOKEN_KEYWORD_PARTIAL     },
         {"private"        ,CodifierTokenType.TT_TOKEN_KEYWORD_PRIVATE     },
         {"protected"      ,CodifierTokenType.TT_TOKEN_KEYWORD_PROTECTED   },
         {"public"         ,CodifierTokenType.TT_TOKEN_KEYWORD_PUBLIC      },
         {"readonly"       ,CodifierTokenType.TT_TOKEN_KEYWORD_READONLY    },
         {"ref"            ,CodifierTokenType.TT_TOKEN_KEYWORD_REF         },
         {"return"         ,CodifierTokenType.TT_TOKEN_KEYWORD_RETURN      },
         {"sbyte"          ,CodifierTokenType.TT_TOKEN_KEYWORD_SBYTE       },
         {"sealed"         ,CodifierTokenType.TT_TOKEN_KEYWORD_SEALED      },
         {"short"          ,CodifierTokenType.TT_TOKEN_KEYWORD_SHORT       },
         {"sizeof"         ,CodifierTokenType.TT_TOKEN_KEYWORD_SIZEOF      },
         {"stackalloc"     ,CodifierTokenType.TT_TOKEN_KEYWORD_STACKALLOC  },
         {"static"         ,CodifierTokenType.TT_TOKEN_KEYWORD_STATIC      },
         {"string"         ,CodifierTokenType.TT_TOKEN_KEYWORD_STRING      },
         {"struct"         ,CodifierTokenType.TT_TOKEN_KEYWORD_STRUCT      },
         {"switch"         ,CodifierTokenType.TT_TOKEN_KEYWORD_SWITCH      },
         {"this"           ,CodifierTokenType.TT_TOKEN_KEYWORD_THIS        },
         {"throw"          ,CodifierTokenType.TT_TOKEN_KEYWORD_THROW       },
         {"true"           ,CodifierTokenType.TT_TOKEN_KEYWORD_TRUE        },
         {"try"            ,CodifierTokenType.TT_TOKEN_KEYWORD_TRY         },
         {"typeof"         ,CodifierTokenType.TT_TOKEN_KEYWORD_TYPEOF      },
         {"uint"           ,CodifierTokenType.TT_TOKEN_KEYWORD_UINT        },
         {"ulong"          ,CodifierTokenType.TT_TOKEN_KEYWORD_ULONG       },
         {"unchecked"      ,CodifierTokenType.TT_TOKEN_KEYWORD_UNCHECKED   },
         {"unsafe"         ,CodifierTokenType.TT_TOKEN_KEYWORD_UNSAFE      },
         {"ushort"         ,CodifierTokenType.TT_TOKEN_KEYWORD_USHORT      },
         {"using"          ,CodifierTokenType.TT_TOKEN_KEYWORD_USING       },
         {"virtual"        ,CodifierTokenType.TT_TOKEN_KEYWORD_VIRTUAL     },
         {"volatile"       ,CodifierTokenType.TT_TOKEN_KEYWORD_VOLATILE    },
         {"void"           ,CodifierTokenType.TT_TOKEN_KEYWORD_VOID        },
         {"while"          ,CodifierTokenType.TT_TOKEN_KEYWORD_WHILE       }};

       }
}
