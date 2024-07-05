namespace Fx.Caching
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Metadata.Ecma335;
    using System.Runtime.InteropServices;
    using Microsoft.Extensions.Caching.Memory;
    using static System.Runtime.InteropServices.JavaScript.JSType;

    public sealed class ObjectCache : IObjectCache
    {
        private readonly IMemoryCache memoryCache;

        public ObjectCache(MemoryCacheFactory memoryCacheFactory)
        {
            //// TODO we are using a factory so that we get our own instance of the memory cache, that way we know that no one has added a null value to the cache; however, the factory may be a closure
            //// TODO using an expression doesn't prevent the creation of the closure

            /*if (memoryCacheFactory == null)
            {
                throw new ArgumentNullException(nameof(memoryCacheFactory));
            }*/

            this.memoryCache = memoryCacheFactory.Create();
        }

        public void CreateEntry(object key, object value)
        {
            using (var entry = this.memoryCache.CreateEntry(key))
            {
                entry.Value = value;
            }
        }

        public void Remove(object key)
        {
            this.memoryCache.Remove(key);
        }

        public bool TryGetValue(object key, out object value)
        {
            //// TODO we don't fully own the memory cache used in this.memoryCache, so someone external to us *could* have actually added a null value; we need to protect against that case
            return this.memoryCache.TryGetValue(key, out value);
        }
    }

    public static class Driver2
    {
        public static void DoWork()
        {
        }
    }

    public sealed class RavudetCache : IObjectCache
    {
        private readonly IMemoryCache memoryCache;

        public RavudetCache(RavudetFactory<IMemoryCache> cacheFactory)
        {
            if (cacheFactory == null)
            {
                throw new ArgumentNullException(nameof(cacheFactory));
            }

            this.memoryCache = cacheFactory.Create();
        }

        public void CreateEntry(object key, object value)
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(object key, out object value)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class NestedMemoryCache : IMemoryCache
    {
        private readonly IMemoryCache memoryCache;

        public NestedMemoryCache(RavudetFactory<IMemoryCache> cacheFactory)
        {
            if (cacheFactory == null)
            {
                throw new ArgumentNullException(nameof(cacheFactory));
            }

            this.memoryCache = cacheFactory.Create();
        }

        public ICacheEntry CreateEntry(object key)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Remove(object key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(object key, out object? value)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class RavudetFactory<T>
    {
        private readonly Func<T> factory;

        public RavudetFactory(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            //// TODO what if no "new"s are called?
            new RavudetFactoryVisitor<T>().Visit(expression);

            this.factory = expression.Compile();
        }

        public T Create()
        {
            return this.factory();
        }
    }

    //// TODO create a visitor that recreates the code of the expression visited

    public sealed class RavudetFactoryVisitor<T> : ExpressionVisitor
    {
        protected override Expression VisitNew(NewExpression node)
        {
            //// TODO there's no guarantee that this is the "new" T that's returned by the factory; somehow, a godoexpression is the return statement: https://learn.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression.return?view=net-8.0 but what about the cases where you *don't* have code blocks and therefore don't have return statements?
            //// TODO what about the case where there's code like:
            ///{
            ///  var builder = new Builder();
            ///  builder.Prop1 = "asdf";
            ///  return builder.Build();
            ///}
            //// TODO the thing about the above code is, you use builders to make the same guarantees that you are trying to get out of the factories; if you have a settings factory method, and the settings class had a bunch of factories as its properties, then this would still work, and you wouldn't need the builder at all; you should maybe try out this paradigm somewhere

            var constructorInfo = node.Constructor; //// TODO when is this null?
            var declaringType = constructorInfo.DeclaringType; //// TODO when is this null?
            if (declaringType.IsAssignableTo(typeof(T))) //// TODO does this always work? try it with something like list<string>
            {
                //// TODO you should now trace node.Arguments back using a dependency tree; you are wanting to check if any references used by the arguments are still being held by the caller, and if any of those references are memory caches; the idea is that you don't care about other references; BUT this is not really the case,  you do care about other references; what if the memory cache implementation uses a dictionary as its backing store and a reference is still held to the dictionary? in that case, someone could add a null valuye to the dicaitonary, and then as a result the memory cache generated would *also* have that null value, so no protection was gained; if you require that there are *no* references to any dependencies, then the memory cachce implementation would need to have a factory for the dictionary, and it would be factories all the way down; maybe this is actually worth it then...
                foreach (var argument in node.Arguments)
                {
                    if (argument.Type.IsPrimitive)
                    {
                        // primitives don't have dangling references
                        continue;
                    }

                    if (argument.Type == typeof(string))
                    {
                        // strings are immutable and interned, so they don't have dangling references
                        continue;
                    }

                    // if it's not a string or a primitive, then it better be a factory
                    if (argument.Type.IsGenericType)
                    {
                        if (argument.Type.GetGenericTypeDefinition() == typeof(RavudetFactory<>))
                        {
                            // woohoo, we are another factory, so everything is good
                            continue;
                        }
                    }

                    if (argument is NewExpression)
                    {
                        // the call to base.visitnew should handle the recursion...
                        continue;
                    }

                    throw new Exception("TODO");
                }
            }

            return base.VisitNew(node);
        }
    }

    public sealed class MemoryCacheFactory
    {
        private readonly Func<IMemoryCache> factory;

        public MemoryCacheFactory(Expression<Func<IMemoryCache>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var visitor = new FactoryVisitor();
            visitor.Visit(expression);
            if (!visitor.NewCalled)
            {
                throw new Exception("TODO");
            }

            this.factory = expression.Compile();
        }

        public IMemoryCache Create()
        {
            return this.factory();
        }
    }

    public sealed class Nonclosure<TDelegate> where TDelegate : Delegate
    {
        public Nonclosure(Expression<TDelegate> expression)
        {
            Expression = expression;
        }

        public Expression<TDelegate> Expression { get; }
    }

    public sealed class FactoryVisitor : ExpressionVisitor
    {
        public bool NewCalled { get; private set; } = false; //// TODO this shouldn't be checked by the caller

        [return: NotNullIfNotNull("node")]
        public override Expression? Visit(Expression? node)
        {
            var visited = base.Visit(node);
            return visited;
        }

        /// <summary>
        /// Visits the children of the <see cref="BinaryExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitBinary(BinaryExpression node)
        {
            return base.VisitBinary(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="BlockExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitBlock(BlockExpression node)
        {
            return base.VisitBlock(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="ConditionalExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitConditional(ConditionalExpression node)
        {
            return base.VisitConditional(node);
        }

        /// <summary>
        /// Visits the <see cref="ConstantExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitConstant(ConstantExpression node)
        {
            return base.VisitConstant(node);
        }

        /// <summary>
        /// Visits the <see cref="DebugInfoExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            return base.VisitDebugInfo(node);
        }

        /// <summary>
        /// Visits the <see cref="DefaultExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitDefault(DefaultExpression node)
        {
            return base.VisitDefault(node);
        }

        /// <summary>
        /// Visits the children of the extension expression.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        /// <remarks>
        /// This can be overridden to visit or rewrite specific extension nodes.
        /// If it is not overridden, this method will call <see cref="Expression.VisitChildren"/>,
        /// which gives the node a chance to walk its children. By default,
        /// <see cref="Expression.VisitChildren"/> will try to reduce the node.
        /// </remarks>
        protected override Expression VisitExtension(Expression node)
        {
            return base.VisitExtension(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="GotoExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitGoto(GotoExpression node)
        {
            return base.VisitGoto(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="InvocationExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitInvocation(InvocationExpression node)
        {
            return base.VisitInvocation(node);
        }

        /// <summary>
        /// Visits the <see cref="LabelTarget"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        [return: NotNullIfNotNull(nameof(node))]
        protected override LabelTarget? VisitLabelTarget(LabelTarget? node)
        {
            return base.VisitLabelTarget(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="LabelExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitLabel(LabelExpression node)
        {
            return base.VisitLabel(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="Expression{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the delegate.</typeparam>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return base.VisitLambda(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="LoopExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitLoop(LoopExpression node)
        {
            return base.VisitLoop(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            return base.VisitMember(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="IndexExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitIndex(IndexExpression node)
        {
            return base.VisitIndex(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="MethodCallExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            return base.VisitMethodCall(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="NewArrayExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            return base.VisitNewArray(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="NewExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitNew(NewExpression node)
        {
            var constructorInfo = node.Constructor; //// TODO when is this null?
            var declaringType = constructorInfo.DeclaringType;
            var interfaces = declaringType.GetInterfaces();
            if (interfaces.Contains(typeof(IMemoryCache))) //// TODO what about when a new memory cache is created, but it's not used in the cache returned from the factory?
            {
                this.NewCalled = true;

                //// TODO you should now trace node.Arguments back using a dependency tree; you are wanting to check if any references used by the arguments are still being held by the caller, and if any of those references are memory caches; the idea is that you don't care about other references; BUT this is not really the case,  you do care about other references; what if the memory cache implementation uses a dictionary as its backing store and a reference is still held to the dictionary? in that case, someone could add a null valuye to the dicaitonary, and then as a result the memory cache generated would *also* have that null value, so no protection was gained; if you require that there are *no* references to any dependencies, then the memory cachce implementation would need to have a factory for the dictionary, and it would be factories all the way down; maybe this is actually worth it then...
                foreach (var argument in node.Arguments)
                {
                    if (argument is NewExpression)
                    {
                        //// TODO implement this
                    }

                    if (!argument.Type.IsValueType)
                    {
                        // TODO if it's a value type, then it's copied, but does that matter? what if the value type is actually just holding a single reference to a reference type? now you have actually just copied a reference type; this shouldn't get callers off the hook
                    }

                    if (!argument.Type.IsPrimitive) //// TODO fix this, primitives are allowed; you should also fix it so that strings are allowed
                    {
                        // if it's not a new instance and it's not a primitive, then it better be a factory
                        if (argument.Type.IsGenericType)
                        {
                            if (argument.Type.GetGenericTypeDefinition() == typeof(RavudetFactory<>))
                            {
                                // woohoo, we are another factory, so everything is good
                                continue;
                            }
                        }
                    }

                    throw new Exception("TODO");
                }
            }

            return base.VisitNew(node);
        }

        /// <summary>
        /// Visits the <see cref="ParameterExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return base.VisitParameter(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="RuntimeVariablesExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return base.VisitRuntimeVariables(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="SwitchCase"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            return base.VisitSwitchCase(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="SwitchExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitSwitch(SwitchExpression node)
        {
            return base.VisitSwitch(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="CatchBlock"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return base.VisitCatchBlock(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="TryExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitTry(TryExpression node)
        {
            return base.VisitTry(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="TypeBinaryExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            return base.VisitTypeBinary(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="UnaryExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitUnary(UnaryExpression node)
        {
            return base.VisitUnary(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberInitExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return base.VisitMemberInit(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="ListInitExpression"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitListInit(ListInitExpression node)
        {
            return base.VisitListInit(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="ElementInit"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override ElementInit VisitElementInit(ElementInit node)
        {
            return base.VisitElementInit(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberBinding"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return base.VisitMemberBinding(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberAssignment"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return base.VisitMemberAssignment(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberMemberBinding"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            return base.VisitMemberMemberBinding(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="MemberListBinding"/>.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            return base.VisitMemberListBinding(node);
        }

        /// <summary>
        /// Visits the children of the <see cref="DynamicExpression" />.
        /// </summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified;
        /// otherwise, returns the original expression.</returns>
        protected override Expression VisitDynamic(DynamicExpression node)
        {
            return base.VisitDynamic(node);
        }
    }
}
