using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace WindowsFormsApp2
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
     class Parse
    {
        

        int InputPointer = 0;
        List<Token> TokenStream;
        public List<String> errors = new List<String>();
        public Node root;
        public static Node treeroot;
 

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Type[] datatypes = { Type._INT, Type._FLOAT, Type._STRING };
        Type[] Opretontypes = { Type.GREATERTHAN, Type.LESSTHAN, Type.EQUALTO, Type.NOTEQUAL };
        Type[] ArthmOptypes = { Type.PLUSOPERATOR, Type.MULTIPLICATIONOPERATOR, Type.MINUSOPERATOR, Type.DIVISIONOPERATOR };

        Node Program()
        {
            Node program = new Node("Program");
            program.Children.Add(FunctionStatements());
            program.Children.Add(Main_function());
            return program;
        }
        private Node FunctionStatements()
        {
            Node FunctionStatement = new Node("FunctionStatements");
            if (InputPointer + 1 < TokenStream.Count && isDatatype(InputPointer) && TokenStream[InputPointer + 1].type != Type.MAIN)
            {
                FunctionStatement.Children.Add(Function_Statement());
                FunctionStatement.Children.Add(FunctionStatements());
                return FunctionStatement;
            }
            else
                return null;
        }


      
        private Node Args()
        {
            Node Args = new Node("Args");
            if (InputPointer < TokenStream.Count && isExpression(InputPointer))
            {
                Args.Children.Add(Expression());
                Args.Children.Add(Ident());
                return Args;
            }
            else
                return null;
        }

        private Node Ident()
        {
            Node idnt = new Node("Ident");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.COMMA)
            {
                idnt.Children.Add(match(Type.COMMA));
                idnt.Children.Add(match(Type.IDENTIFIER));
                idnt.Children.Add(Ident());
                return idnt;
            }
            else
                return null;
        }
        private Node FunctionCall()
        {
            Node funcCall = new Node("Function Call");
          
            funcCall.Children.Add(match(Type.IDENTIFIER));
            funcCall.Children.Add(match(Type.LEFTPARENTHESES));
            funcCall.Children.Add(Args());
            funcCall.Children.Add(match(Type.RIGHTPARENTHESES));
            return funcCall;
        }
        Node Assignment_statement()
        {
            Node assign = new Node("Assignment_statement");
            assign.Children.Add(match(Type.IDENTIFIER));
            assign.Children.Add(match(Type.ASSIGNMENTOPERATOR));
            assign.Children.Add(Expression());
            return assign;
        }
      

        private Node Term()
        {
            Node term = new Node("Term");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.NUMBER)
            {
                term.Children.Add(match(Type.NUMBER));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.IDENTIFIER)
            {
                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].type == Type.LEFTPARENTHESES)
                {
                    term.Children.Add(FunctionCall());
                }
                else
                    term.Children.Add(match(Type.IDENTIFIER));
            }
          
            return term;
        }
        private Node Equation()
        {
            Node equation = new Node("Equation");
            equation.Children.Add(Terms());
            equation.Children.Add(SubEquation());
            return equation;
        }

        private Node Terms()
        {
            Node terms = new Node("Terms");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.LEFTPARENTHESES)
            {
                terms.Children.Add(match(Type.LEFTPARENTHESES));
                terms.Children.Add(Equation());
                terms.Children.Add(match(Type.RIGHTPARENTHESES));
                terms.Children.Add(SubEquationMD());
            }
            else if (InputPointer < TokenStream.Count && isTerm(InputPointer))
            {
                terms.Children.Add(Term());
                terms.Children.Add(SubEquationMD());
            }
           
            return terms;
        }
        private Node SubEquationMD()
        {
            Node subEquation = new Node("SubEquationMDOrDiv");
            if (InputPointer < TokenStream.Count && isMultOp(InputPointer))
            {
                subEquation.Children.Add(MultOp());
                subEquation.Children.Add(Equation());
                subEquation.Children.Add(SubEquationMD());
                return subEquation;
            }
            else
                return null;
        }

        private Node SubEquation()
        {
            Node subEquation = new Node("SubEquationAddOrMIn");
            if (InputPointer < TokenStream.Count && isAddOp(InputPointer))
            {
                subEquation.Children.Add(AddOp());
                subEquation.Children.Add(Terms());
                subEquation.Children.Add(SubEquation());
                return subEquation;
            }
            else
                return null;
        }

        private Node AddOp()
        {
            Node addOp = new Node("Add Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.PLUSOPERATOR)
            {
                addOp.Children.Add(match(Type.PLUSOPERATOR));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.MINUSOPERATOR)
            {
                addOp.Children.Add(match(Type.MINUSOPERATOR));
            }
             
            return addOp;
        }
        private Node MultOp()
        {
            Node multOp = new Node("Mult Operator");
            
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.DIVISIONOPERATOR)
            {
                multOp.Children.Add(match(Type.DIVISIONOPERATOR));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.MULTIPLICATIONOPERATOR)
            {
                multOp.Children.Add(match(Type.MULTIPLICATIONOPERATOR));
            }

            return multOp;
        }

        private Node Expression()
        {
            Node exp = new Node("Expression");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.STRING)
            {
                exp.Children.Add(match(Type.STRING));
            }
            else if (InputPointer < TokenStream.Count && isEquation(InputPointer))
            {
                exp.Children.Add(Equation());
            }
            
            return exp;
        }
        private Node Datatype()
        {
            Node datatype = new Node("Datatype");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type._INT)
            {
                datatype.Children.Add(match(Type._INT));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type._FLOAT)
            {
                datatype.Children.Add(match(Type._FLOAT));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type._STRING)
            {
                datatype.Children.Add(match(Type._STRING));
            }
            
            return datatype;
        }
        Node Declaration_Statement()
        {
            Node decs = new Node("Declaration_Statement");
            decs.Children.Add(Datatype());
            decs.Children.Add(IdsList());
            decs.Children.Add(match(Type.SEMICOLON));
            return decs;
        }
        Node IdsList()
        {
            Node id = new Node("IdsList");
            if (InputPointer + 1 < TokenStream.Count && (TokenStream[InputPointer + 1].type == Type.COMMA || TokenStream[InputPointer + 1].type == Type.SEMICOLON))
            {
                id.Children.Add(match(Type.IDENTIFIER));
                id.Children.Add(Ids());
            }
            else if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].type == Type.ASSIGNMENTOPERATOR)
            {
                id.Children.Add(Assignment_statement());
                id.Children.Add(Ids());
            }
            
            return id;
        }
        Node Ids()
        {
            Node id = new Node("Ids");

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.COMMA)
            {
                id.Children.Add(match(Type.COMMA));
                id.Children.Add(IdsList());
                return id;
            }
            else
                return null;

        }
        Node Read_Statement()
        {
            Node read = new Node("Read_Statement");
            read.Children.Add(match(Type.READ));
            read.Children.Add(match(Type.IDENTIFIER));
            read.Children.Add(match(Type.SEMICOLON));
            return read;
        }
        Node w_state()
        {
            Node write = new Node("w_state");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.ENDLINE)
            {
                write.Children.Add(match(Type.ENDLINE));
            }
            else if (InputPointer < TokenStream.Count && isExpression(InputPointer))
            {
                write.Children.Add(Expression());
            }
            
            return write;
        }
        Node Write_Statement()
        {
            Node writeS = new Node("Write Statement");
            writeS.Children.Add(match(Type.WRITE));
            writeS.Children.Add(w_state());
            writeS.Children.Add(match(Type.SEMICOLON));
            return writeS;
        }
        private Node AssignmentStatment()
        {
            Node assign = new Node("Assignment Statment");
            assign.Children.Add(match(Type.IDENTIFIER));
            assign.Children.Add(match(Type.ASSIGNMENTOPERATOR));
            assign.Children.Add(Expression());
            return assign;
        }

        //####################################

        private Node Return_Statement()
        {
            Node returnstatement = new Node("Return Statement");
            returnstatement.Children.Add(match(Type.RETURN));
            returnstatement.Children.Add(Expression());
            returnstatement.Children.Add(match(Type.SEMICOLON));

            return returnstatement;
        }
        Node Condition_Operator ()
        {
            Node Condition_Operator  = new Node("Condition Operator");

            if (TokenStream[InputPointer].type == Type.LESSTHAN)
            {
                Condition_Operator .Children.Add(match(Type.LESSTHAN));

            }
            else if (TokenStream[InputPointer].type == Type.GREATERTHAN)
            {
                Condition_Operator .Children.Add(match(Type.GREATERTHAN));

            }
            else if (TokenStream[InputPointer].type == Type.EQUALTO)
            {
                Condition_Operator .Children.Add(match(Type.EQUALTO));

            }
            else if (TokenStream[InputPointer].type == Type.NOTEQUAL)
            {
                Condition_Operator .Children.Add(match(Type.NOTEQUAL));

            }
           
            return Condition_Operator ;
        }
        Node Condition()
        {
            Node condition = new Node("Condition");

            condition.Children.Add(match(Type.IDENTIFIER));
            condition.Children.Add(Condition_Operator());
            condition.Children.Add(Term());

            return condition;
        }

        private Node Boolean_Operator()
        {
            Node boolOpr = new Node("Boolean Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.AND)
            {
                boolOpr.Children.Add(match(Type.AND));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.OR)
            {
                boolOpr.Children.Add(match(Type.OR));
            }
            
            return boolOpr;
        }
        Node Condition_Statement()
        {
            Node conditionstatement = new Node("Condition Statement");

            conditionstatement.Children.Add(Condition());
            conditionstatement.Children.Add(b_state());

            return conditionstatement;
        }

        Node b_state()
        {
            Node b_stat = new Node("b_state");
            if (isBoolean_Operator(InputPointer))
            {
                b_stat.Children.Add(Boolean_Operator());
                b_stat.Children.Add(Condition());
                b_stat.Children.Add(b_state());

                return b_stat;
            }
            else
                return null;
        }
      

        private Node Statements()
        {
            Node statements = new Node("Statements");
            if (InputPointer < TokenStream.Count && isStatement(InputPointer))
            {
                statements.Children.Add(Statement());
                statements.Children.Add(Statements());
                return statements;
            }
            else
                return null;
        }

    

        private Node Statement()
        {
            Node statement = new Node("Statement");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.IDENTIFIER)
            {
                
                if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].type == Type.ASSIGNMENTOPERATOR)
                {
                    statement.Children.Add(AssignmentStatment());
                    statement.Children.Add(match(Type.SEMICOLON));
                }
               
                else if (InputPointer + 1 < TokenStream.Count && isCondition_Operator (InputPointer + 1))
                {
                    statement.Children.Add(Condition_Statement());
                }
               
                else if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1].type == Type.LEFTPARENTHESES)
                {
                    statement.Children.Add(FunctionCall());
                    statement.Children.Add(match(Type.SEMICOLON));
                }
                
            }
           
            else if (InputPointer < TokenStream.Count && isDatatype(InputPointer))
            {
                statement.Children.Add(Declaration_Statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.WRITE)
            {
                statement.Children.Add(Write_Statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.READ)
            {
                statement.Children.Add(Read_Statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.IF)
            {
                statement.Children.Add(If_Statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.REPEATSTATEMENT)
            {
                statement.Children.Add(Repeat_Statement());
            }
            
            return statement;
        }

        Node State()
        {
            Node state = new Node("State");
            if (InputPointer < TokenStream.Count && isStatement(InputPointer))
            {
                state.Children.Add(Statements());
                state.Children.Add(State());
                return state;
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.RETURN)
            {
                state.Children.Add(Return_Statement());
                return state;
            }
            else
                return null;
        }

        Node If_Statement()
        {

            Node ifst = new Node("If_Statement");

            ifst.Children.Add(match(Type.IF));
            ifst.Children.Add(Condition_Statement());
            ifst.Children.Add(match(Type.THEN));
            ifst.Children.Add(State());
            ifst.Children.Add(next_state ());


            return ifst;

        }
        Node Else_If_Statement()
        {

            Node ElseIf = new Node("else_if_statement");

            ElseIf.Children.Add(match(Type.ELSEIF));
            ElseIf.Children.Add(Condition_Statement());
            ElseIf.Children.Add(match(Type.THEN));
            ElseIf.Children.Add(State());
            ElseIf.Children.Add(next_state ());
            return ElseIf;

        }
        Node next_state()
        {

            Node nextstate = new Node("next_state");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.ELSEIF)
            {
                nextstate.Children.Add(Else_If_Statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.ELSE)
            {
                nextstate.Children.Add(Else_statement());
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.END)
            {
                nextstate.Children.Add(match(Type.END));
            }
            // else
            // printError("End");
            return nextstate;

        }


        Node Else_statement()
        {

            Node elseState = new Node("ElseState");

            elseState.Children.Add(match(Type.ELSE));
            elseState.Children.Add(State());
            elseState.Children.Add(match(Type.END));

            return elseState;

        }
        private Node Repeat_Statement()
        {
            Node repeatStatement = new Node("Repeat Statement");
            repeatStatement.Children.Add(match(Type.REPEATSTATEMENT));
            repeatStatement.Children.Add(Statements());
            repeatStatement.Children.Add(match(Type.UNTIL));
            repeatStatement.Children.Add(Condition_Statement());
            return repeatStatement;
        }
        private Node Parameter()
        {
            Node parameter = new Node("Parameter");
            parameter.Children.Add(Datatype());
            parameter.Children.Add(match(Type.IDENTIFIER));
            return parameter;
        }
        //####################################
        private Node Function_Declaration()
        {

            Node functiondeclaration = new Node("Function Declaration ");
            functiondeclaration.Children.Add(Datatype());
            functiondeclaration.Children.Add(match(Type.IDENTIFIER));
            functiondeclaration.Children.Add(match(Type.LEFTPARENTHESES));
            functiondeclaration.Children.Add(Parameters());
            functiondeclaration.Children.Add(match(Type.RIGHTPARENTHESES));


            return functiondeclaration;

        }

        private Node Parameters()
        {
            Node paramet = new Node("Parameters");
            if (InputPointer < TokenStream.Count && isDatatype(InputPointer))
            {
                paramet.Children.Add(Parameter());
                paramet.Children.Add(Param());
                return paramet;


            }
            else
                return null;

        }
        private Node Param()
        {
            Node parameterD = new Node("Parameter'");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer].type == Type.COMMA)
            {
                parameterD.Children.Add(match(Type.COMMA));
                parameterD.Children.Add(Parameter());
                parameterD.Children.Add(Param());
                return parameterD;
            }
            else
                return null;
        }
        private Node Function_Body()
        {


            Node functionbody = new Node("Function Body");

            functionbody.Children.Add(match(Type.LEFTCURLYBRACKETS));
            functionbody.Children.Add(Statements());
            functionbody.Children.Add(Return_Statement());
            functionbody.Children.Add(match(Type.RIGHTCURLYBRACKETS));

            return functionbody;
        }
        private Node Function_Statement()
        {
            Node functionStatement = new Node("Function Statement");
            functionStatement.Children.Add(Function_Declaration());
            functionStatement.Children.Add(Function_Body());
            return functionStatement;
        }
        private Node Main_function()
        {
            Node mainfunction = new Node("Main function");
            mainfunction.Children.Add(Datatype());
            mainfunction.Children.Add(match(Type.MAIN));
            mainfunction.Children.Add(match(Type.LEFTPARENTHESES));
            mainfunction.Children.Add(match(Type.RIGHTPARENTHESES));
            mainfunction.Children.Add(Function_Body());
            return mainfunction;
        }
        public Node match(Type ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer].type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());
                    return newNode;

                }

                else
                {
                    errors.Add("Parsing Error11: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer].type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
               errors.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }

        private bool isDatatype(int InputPointer)
        {
            bool isInt = TokenStream[InputPointer].type == Type._INT;
            bool isFloat = TokenStream[InputPointer].type == Type._FLOAT;
            bool isString = TokenStream[InputPointer].type == Type._STRING;
            return (isInt || isFloat || isString);
        }

        private bool isStatement(int InputPointer)
        {
            bool isDecleration = isDatatype(InputPointer);
            bool isWrite = TokenStream[InputPointer].type == Type.WRITE;
            bool isRead = TokenStream[InputPointer].type == Type.READ;
            bool isConditionOrFunctionCallOrAssignment = TokenStream[InputPointer].type == Type.IDENTIFIER;
            bool isIf = TokenStream[InputPointer].type == Type.IF;
            bool isRepeat = TokenStream[InputPointer].type == Type.REPEATSTATEMENT;
            return (isDecleration || isWrite || isRead || isConditionOrFunctionCallOrAssignment || isIf || isRepeat);
        }
        private bool isCondition_Operator(int InputPointer)
        {
            bool isLessThan = TokenStream[InputPointer].type == Type.LESSTHAN;
            bool isGreaterThan = TokenStream[InputPointer].type == Type.GREATERTHAN;
            bool isEqual = TokenStream[InputPointer].type == Type.EQUALTO;
            bool isNotEqual = TokenStream[InputPointer].type == Type.NOTEQUAL;
            return (isEqual || isGreaterThan || isLessThan || isNotEqual);
        }

        private bool isBoolean_Operator(int InputPointer)
        {
            bool isOr = TokenStream[InputPointer].type == Type.OR;
            bool isAnd = TokenStream[InputPointer].type == Type.AND;
            return (isAnd || isOr);
        }
        private bool isTerm(int InputPointer)
        {
            bool isNUMBER = TokenStream[InputPointer].type == Type.NUMBER;
            bool isIdentifier = TokenStream[InputPointer].type == Type.IDENTIFIER;

            return (isNUMBER || isIdentifier);
        }
        private bool isAddOp(int InputPointer)
        {
            bool isPlus = TokenStream[InputPointer].type == Type.PLUSOPERATOR;
            bool isMinus = TokenStream[InputPointer].type == Type.MINUSOPERATOR;
            return isPlus || isMinus;
        }
        private bool isMultOp(int InputPointer)
        {
            bool isMult = TokenStream[InputPointer].type == Type.MULTIPLICATIONOPERATOR;
            bool isDivide = TokenStream[InputPointer].type == Type.DIVISIONOPERATOR;
            return isMult || isDivide;
        }
        private bool isEquation(int InputPointer)
        {
            bool isLEFTPARENTHESES = TokenStream[InputPointer].type == Type.LEFTPARENTHESES;
            return (isLEFTPARENTHESES || isTerm(InputPointer));
        }
        private bool isExpression(int InputPointer)
        {
            bool isString = TokenStream[InputPointer].type == Type.STRING;//
            return (isString || isEquation(InputPointer));
        }
    }
}
