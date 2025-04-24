// See https://aka.ms/new-console-template for more information
internal class CustomContainer
{
    private readonly Dictionary<Type, Type> _iocContainer = new Dictionary<Type, Type>();
        
    internal void Register<TFrom, TTo>()
    {
        Register(typeof(TFrom), typeof(TTo));
    }

    private void Register(Type TFrom, Type TTo) //Register(ICreditCard, MasterCard)
    {
        if (!_iocContainer.ContainsKey(TFrom))
        {
            // Allow only adding unique type with no override
            _iocContainer.Add(TFrom, TTo);
        }
        else
        {
            // Allow overriding the types on registeration 
            _iocContainer[TFrom] = TTo;

            // If override not allowed will always use first registration
            //_iocContainer[TTo] = _iocContainer[TFrom]; /// or we can ignore else block
        }
    }

    internal T Resolve<T>()
    {
        return (T)Resolve(typeof(T));
    }

    private object Resolve(Type type) // Resolve(ICreditCard)
    {
        var objType = _iocContainer[type];
            
        return Activator.CreateInstance(objType); // Using Default Construtor
    }
}