using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Epico
{
    public delegate T ObjectActivator<T>(params object[] args);

    public static class Interno
    {
        /*
         * Activator.CreateInstance: 8.74 sec
         * Linq Expressions:         0.104 sec
         */

        public static T GetActivator<T>(Type type, int posCtor = 0)
        {
            ConstructorInfo ctor = type.GetConstructors().Skip(posCtor).First();

            Type dType = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            // Criar um único parâmetro do tipo type []
            ParameterExpression param =
                Expression.Parameter(typeof(object[]), "args");

            Expression[] argsExp =
                new Expression[paramsInfo.Length];

            // Escolha cada arg da matriz params e crie uma expressão digitada deles
            for (int i = 0; i < paramsInfo.Length; i++)
            {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;

                Expression paramAccessorExp =
                    Expression.ArrayIndex(param, index);

                Expression paramCastExp =
                    Expression.Convert(paramAccessorExp, paramType);

                argsExp[i] = paramCastExp;
            }

            // Faça um NewExpression que chame o ctor com os args que acabamos de criar
            NewExpression newExp = Expression.New(ctor, argsExp);

            // Crie um lambda com Expressão New como corpo e nosso objeto de parâmetro[] como arg
            LambdaExpression lambda =
                Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

            // Compile-o
            ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();
            return compiled.Invoke();
        }
    }
}
