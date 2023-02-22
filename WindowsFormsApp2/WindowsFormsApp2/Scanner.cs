using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public enum Type
    {
        _INT, _FLOAT, _STRING, IF, ELSE, ELSEIF, UNTIL, READ, RETURN, WRITE, ENDLINE, END, CONSTANT, PROGRAM, REPEATSTATEMENT, ASSIGNMENTOPERATOR, IDENTIFIER, SEMICOLON, THEN,
        PLUSOPERATOR, MINUSOPERATOR, EQUALTO, MULTIPLICATIONOPERATOR, LESSTHAN, DIVISIONOPERATOR, GREATERTHAN, LEFTCURLYBRACKETS, RIGHTCURLYBRACKETS,
        LEFTPARENTHESES, RIGHTPARENTHESES, COMMA, NOTEQUAL, AND, OR, COMMENT, MAIN, ERROR, NUMBER, STRING, FUNCTIONCALL, FUNCTIONDECLARATION
    }
    class Token
    {
        public string input;
        public Type type;
        public Token() { } //constractor mlnash da3wa beeh
        public Token(string Input, Type Type)
        {
            this.input = Input;
            this.type = Type;
        }


    }
    class Scanner
    {
        public Scanner() { }
        
       
        public Dictionary<string, Type> reservedWords = new Dictionary<string, Type>
        {
            {"int",Type._INT},
            {"float",Type._FLOAT},
            {"string",Type._STRING},
            {"if",Type.IF},
            {"else",Type.ELSE},
            {"elseif",Type.ELSEIF},
            {"until",Type.UNTIL},
            {"read",Type.READ},
            {"return",Type.RETURN},
            {"endl",Type.ENDLINE},
            {"end",Type.END},
            {"constant",Type.CONSTANT},
            {"program",Type.PROGRAM},
            {"repeat",Type.REPEATSTATEMENT},
            {"write",Type.WRITE},
            {"then",Type.THEN},
            {"main",Type.MAIN}
        };
        public Dictionary<string, Type> operators = new Dictionary<string, Type>{
            {";",Type.SEMICOLON},
            {":=",Type.ASSIGNMENTOPERATOR},
            {"<>",Type.NOTEQUAL},
            {"&&",Type.AND},
            {"||",Type.OR},
            {"main",Type.MAIN},
            {"+",Type.PLUSOPERATOR},
            {"-",Type.MINUSOPERATOR},
            {"*",Type.MULTIPLICATIONOPERATOR},
            {"<",Type.LESSTHAN},
            {"/",Type.DIVISIONOPERATOR},
            {">",Type.GREATERTHAN},
            {"=",Type.EQUALTO},
            {"{",Type.LEFTCURLYBRACKETS},
            {"(",Type.LEFTPARENTHESES},
            {")",Type.RIGHTPARENTHESES},
            {"}", Type.RIGHTCURLYBRACKETS},
            {",",Type.COMMA},
            };
        public Type findTokens(string s)  // return type for lexeme
        {
            if (reservedWords.ContainsKey(s))
            {
                return reservedWords[s];
            }
            else
                return Type.IDENTIFIER;
        }
 
        public List<Token> tokens = new List<Token>();
        public  List<Token> error = new List<Token>();
        public List<Token> newlines = new List<Token>();


        public string lexeme = "";
        public Token lastToken;
        public void AddTokenToList(string str, Type type)
        {
            Token to = new Token(str, type);
            if (type == Type.ERROR)
                error.Add(to);
            else 
                tokens.Add(to);


            lastToken = new Token(str, type);
            lexeme = "";
        }
        public void StartScanner(string tinyCode)
        {
            for (int i = 0; i < tinyCode.Length; i++)
            {
                // number
                if (Char.IsDigit(tinyCode[i]))
                {

                    int j = i;
                    while (j < tinyCode.Length && (Char.IsDigit(tinyCode[j]) || tinyCode[j] == '.'))
                    {
                        lexeme += tinyCode[j];
                        j++;
                    }
                    //bouns
                   
                    if (j < tinyCode.Length && Char.IsLetter(tinyCode[j]))
                    {
                        while (j < tinyCode.Length && Char.IsLetterOrDigit(tinyCode[j]))
                        {
                            lexeme += tinyCode[j];
                            j++;
                        }
                        AddTokenToList(lexeme, Type.ERROR);
                    }
                    else if (lexeme[lexeme.Length - 1] == '.')
                    {
                        AddTokenToList(lexeme, Type.ERROR);
                    }

                    else if (lexeme.Count(x => x == '.') > 1)
                    {
                        AddTokenToList(lexeme, Type.ERROR);
                    }
                    else
                    {
                        AddTokenToList(lexeme, Type.NUMBER);
                    }
                        i = j - 1;

                }

               else if ((tinyCode[i])=='.' && i+1 < tinyCode.Length && Char.IsDigit(tinyCode[i+1]) )
               {

                    int j = i;
                    while (j < tinyCode.Length && (Char.IsDigit(tinyCode[j]) || tinyCode[j] == '.'))
                    {
                        lexeme += tinyCode[j];
                        j++;
                    }
                    AddTokenToList(lexeme, Type.ERROR);
                    //bouns
                    i = j - 1;
               }


                //idendifier
                else if (Char.IsLetter(tinyCode[i]) )
                {
                    int j = i;
                    while (j < tinyCode.Length && (Char.IsLetterOrDigit(tinyCode[j])))
                    {
                        lexeme += tinyCode[j];
                        j++;
                    }
                    AddTokenToList(lexeme, findTokens(lexeme));
                    i = j - 1;
                }

                //string

                else if (tinyCode[i] == '"')
                {
                    int j = i;
                    do
                    {
                        lexeme += tinyCode[j];
                        j++;
                    } while (j < tinyCode.Length && tinyCode[j] != '"');
                    if (j == tinyCode.Length && tinyCode[j - 1] != '"')
                    {
                        AddTokenToList(lexeme, Type.ERROR);
                    }
                
                    else if (j == i + 1 && tinyCode[j - 1] == '"')
                    {
                        AddTokenToList('"'.ToString(), Type.ERROR);

                    }
                    else
                    {
                        lexeme += tinyCode[j];
                        AddTokenToList(lexeme, Type.STRING);
                    }
                    i = j;
                }

                //comment 
                else if (i + 1 < tinyCode.Length && (tinyCode[i] == '/' && tinyCode[i + 1] == '*'))
                {
                    int j = i;
                    lexeme += tinyCode[j];
                    lexeme += tinyCode[j + 1];
                    j += 2;
                    while (j + 1 < tinyCode.Length && tinyCode[j].ToString() + tinyCode[j + 1].ToString() != "*/")
                    {
                        lexeme += tinyCode[j];
                        j++;  /* * */
                    }
                    if (j + 1 == tinyCode.Length)
                    {
                        lexeme += tinyCode[j];
                        AddTokenToList(lexeme, Type.ERROR);
                        i = j;
                    }
                    else
                    {
                        lexeme += tinyCode[j];
                        lexeme += tinyCode[j + 1];
                        AddTokenToList(lexeme, Type.COMMENT); i = j + 1;
                    }
                }

              
                else if (tinyCode[i] == '|' && i + 1 < tinyCode.Length && (tinyCode[i + 1] == '|'))
                {
                    lexeme += "||";
                    AddTokenToList(lexeme, Type.OR); 
                    i++;
                }
                else if (tinyCode[i] == '&' && i + 1 < tinyCode.Length && (tinyCode[i + 1] == '&'))
                {
                    lexeme += "&&";
                    AddTokenToList(lexeme, Type.AND);
                    i++;
                }
                else if (tinyCode[i] == ':' && i + 1 < tinyCode.Length && (tinyCode[i + 1] == '='))
                {
                    lexeme += ":=";
                    AddTokenToList(lexeme, Type.ASSIGNMENTOPERATOR);
                    i++;
                }
                else if (tinyCode[i] == '<' && i + 1 < tinyCode.Length && (tinyCode[i + 1] == '='))
                {
                    lexeme += "<=";
                    AddTokenToList(lexeme, Type.ERROR);
                    i++;
                }
                else if (tinyCode[i] == '>' && i + 1 < tinyCode.Length && (tinyCode[i + 1] == '='))
                {
                    lexeme += "<=";
                    AddTokenToList(lexeme, Type.ERROR);
                    i++;
                }
               else if (tinyCode[i] == '<' && i + 1 < tinyCode.Length && (tinyCode[i + 1] == '>')) 
               {
                    lexeme += "<>";
                    AddTokenToList(lexeme, Type.NOTEQUAL);
                    i++; 
               }
 
                
                else if (tinyCode[i] == ' ' || tinyCode[i] == '\n')
                {
                    if (tinyCode[i] == '\n') ;
                }

                else if (operators.ContainsKey(tinyCode[i].ToString()))
                {

                    lexeme += tinyCode[i];
                    AddTokenToList(lexeme, operators[tinyCode[i].ToString()]);

                }

                else
                {
                    lexeme += tinyCode[i];
                    AddTokenToList(lexeme, Type.ERROR);
                }

            }
        }

    }
}
