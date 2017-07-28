module Voiteq.ExpressionUtils.Stringifier

open System.Linq.Expressions

exception InferrerException of string

let raiseUnrecognisedExpressionException (e:Expression) = raise (InferrerException(sprintf "Expression %A of type %A is unrecognised" e e.NodeType))

let isCommutative(e : BinaryExpression) = 
    match e.NodeType with
    | ExpressionType.Add
    | ExpressionType.AddChecked
    | ExpressionType.Multiply
    | ExpressionType.MultiplyChecked
    | ExpressionType.And
    | ExpressionType.Or
    | ExpressionType.ExclusiveOr
    | ExpressionType.AndAlso
    | ExpressionType.OrElse
    | ExpressionType.Equal
    | ExpressionType.NotEqual -> true
    | ExpressionType.Divide
    | ExpressionType.Modulo
    | ExpressionType.Power
    | ExpressionType.Subtract
    | ExpressionType.SubtractChecked
    | ExpressionType.LeftShift
    | ExpressionType.RightShift
    | ExpressionType.GreaterThanOrEqual
    | ExpressionType.GreaterThan
    | ExpressionType.LessThan
    | ExpressionType.LessThanOrEqual
    | ExpressionType.Coalesce
    | ExpressionType.ArrayIndex -> false
    | _ -> raiseUnrecognisedExpressionException e
    
let coalesce a b = if a = null then "" else a + b

let inferCacheKey (predicate: System.Linq.Expressions.Expression<System.Func<'a, bool>>) : string =

    if Seq.length(predicate.Parameters) <> 1 then raise (InferrerException("predicate should have exactly one parameter"))
    let lambdaParameter = predicate.Parameters |> Seq.head
    
    let rec hasBoundCallRoot (e : System.Linq.Expressions.Expression) = 
        match e.NodeType with
        | ExpressionType.Call -> hasBoundCallRoot (e :?> MethodCallExpression).Object
        | ExpressionType.MemberAccess -> hasBoundCallRoot (e :?> MemberExpression).Expression
        | ExpressionType.Parameter -> lambdaParameter <> (e :?> ParameterExpression)
        | ExpressionType.Constant -> true
        | _ -> raiseUnrecognisedExpressionException e

    let rec sortAndStandardiseBinaryExpressionSides (binaryExpression: BinaryExpression) =
        let (left, right) = (binaryExpression.Left, binaryExpression.Right)
        let leftString = standardiseExpression left
        let rightString = standardiseExpression right
        if not (isCommutative binaryExpression) then (leftString, rightString)
        elif left.NodeType = ExpressionType.Constant && right.NodeType <> ExpressionType.Constant then (rightString, leftString)
        elif right.NodeType = ExpressionType.Constant && left.NodeType <> ExpressionType.Constant then (leftString, rightString)
        elif (left.NodeType = ExpressionType.Call || left.NodeType = ExpressionType.MemberAccess) && hasBoundCallRoot left then (rightString, leftString)
        elif (right.NodeType = ExpressionType.Call || right.NodeType = ExpressionType.MemberAccess) && hasBoundCallRoot right then (leftString, rightString)
        elif System.String.CompareOrdinal (leftString, rightString) <= 0 then (leftString, rightString)
        else (rightString, leftString)
        
    and friendlyGenericName (t: System.Type) =
        if (t.Name.StartsWith("Nullable")) then (t.GenericTypeArguments.[0].Name + "?") else t.Name

    and binaryExpressionFormatter (e : System.Linq.Expressions.Expression) binaryCombinator = 
        let binaryExpression = e :?> BinaryExpression
        let (left, right) = sortAndStandardiseBinaryExpressionSides binaryExpression
        "(" + left + binaryCombinator + right + ")"

    and standardiseExpression (e : System.Linq.Expressions.Expression) : string = 
        match e.NodeType with
        | ExpressionType.Lambda -> standardiseExpression (e :?> LambdaExpression).Body
        | ExpressionType.Constant -> e.ToString()
        | ExpressionType.MemberAccess ->
            let memberExpression = e :?> MemberExpression
            if not (hasBoundCallRoot e) then (coalesce (standardiseExpression memberExpression.Expression) ".") + memberExpression.Member.Name
            else Expression.Constant(Expression.Lambda(memberExpression).Compile().DynamicInvoke()).ToString();
        | ExpressionType.And
        | ExpressionType.Or
        | ExpressionType.ExclusiveOr
        | ExpressionType.Power
        | ExpressionType.Modulo
        | ExpressionType.LeftShift
        | ExpressionType.RightShift
        | ExpressionType.Coalesce
        | ExpressionType.ArrayIndex -> binaryExpressionFormatter e ("-" + e.NodeType.ToString() + "-")
        | ExpressionType.GreaterThan -> binaryExpressionFormatter e ">"
        | ExpressionType.GreaterThanOrEqual -> binaryExpressionFormatter e ">="
        | ExpressionType.LessThan -> binaryExpressionFormatter e "<"
        | ExpressionType.LessThanOrEqual -> binaryExpressionFormatter e "<="
        | ExpressionType.Add
        | ExpressionType.AddChecked -> binaryExpressionFormatter e "+"
        | ExpressionType.Divide -> binaryExpressionFormatter e "/"
        | ExpressionType.Multiply
        | ExpressionType.MultiplyChecked -> binaryExpressionFormatter e "*"
        | ExpressionType.Subtract
        | ExpressionType.SubtractChecked -> binaryExpressionFormatter e "-"
        | ExpressionType.AndAlso -> binaryExpressionFormatter e "&&"
        | ExpressionType.OrElse -> binaryExpressionFormatter e "||"
        | ExpressionType.Equal -> binaryExpressionFormatter e "=="
        | ExpressionType.NotEqual -> binaryExpressionFormatter e "!="
        | ExpressionType.Call ->
            let callExp = e :?> MethodCallExpression
            if not (hasBoundCallRoot e) then "(" + (standardiseExpression callExp.Object) + "." + callExp.Method.Name + "(" + (callExp.Arguments |> Seq.map standardiseExpression |> String.concat ",") + "))"
            else Expression.Constant(Expression.Lambda(callExp).Compile().DynamicInvoke()).ToString()
        | ExpressionType.Parameter -> null
        | ExpressionType.Convert -> 
            let convertExpression = e :?> UnaryExpression
            "Convert(" + (standardiseExpression convertExpression.Operand) + ", " + (friendlyGenericName convertExpression.Type) + ")" 
        | _ -> raiseUnrecognisedExpressionException e

    standardiseExpression predicate
