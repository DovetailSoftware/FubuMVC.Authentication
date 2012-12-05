using System;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Authentication
{
    public class AuthenticationNode : Node<AuthenticationNode, AuthenticationChain>, IContainerModel
    {
        private readonly Type _authType;

        public AuthenticationNode(Type authType)
        {
            if (!authType.CanBeCastTo<IAuthenticationStrategy>())
            {
                throw new ArgumentOutOfRangeException("authType", "authType must be assignable to IAuthenticationStrategy");
            }

            _authType = authType;
        }

        public Type AuthType
        {
            get { return _authType; }
        }

        ObjectDef IContainerModel.ToObjectDef()
        {
            var def = new ObjectDef(_authType);

            configure(def);

            return def;
        }

        protected virtual void configure(ObjectDef def)
        {

        }
    }
}