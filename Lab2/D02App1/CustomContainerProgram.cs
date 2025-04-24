using Autofac;
using Lamar;
using System;

namespace D02App1;
public class CustomContainerProgram
{
    static void Main(string[] args)
    {
        Console.WriteLine("========== Using Autofac ==========");
        UseAutofac();

        Console.WriteLine("\n========== Using Lamar ==========");
        UseLamar();
    }

    static void UseAutofac()
    {
        // 1. Create Autofac container builder
        var builder = new ContainerBuilder();

        // 2. Registration for types
        builder.RegisterType<MasterCard>().As<ICreditCard>();
        builder.RegisterType<Shopper>().AsSelf();

        // 3. Build the container
        var autofacContainer = builder.Build();

        using (var scope = autofacContainer.BeginLifetimeScope())
        {
            Console.WriteLine("Using MasterCard with Autofac:");
            var card1 = scope.Resolve<ICreditCard>();
            var client1 = scope.Resolve<Shopper>();
            client1.Checkout(card1);

            using (var childScope = scope.BeginLifetimeScope(childBuilder =>
            {
                childBuilder.RegisterType<VisaCard>().As<ICreditCard>().InstancePerLifetimeScope();
            }))
            {
                Console.WriteLine("\nUsing VisaCard with Autofac:");
                var card2 = childScope.Resolve<ICreditCard>();
                var client2 = childScope.Resolve<Shopper>();
                client2.Checkout(card2);
            }
        }
    }

    static void UseLamar()
    {
        // Using Lamar for Dependency Injection
        var lamarContainer = new Lamar.Container(c =>
        {
            // Register both card types with names
            c.For<ICreditCard>().Use<MasterCard>().Named("mastercard");
            c.For<ICreditCard>().Add<VisaCard>().Named("visa");

            // Register Shopper
            c.For<Shopper>().Use<Shopper>();
        });

        // Get MasterCard instance by name
        Console.WriteLine("Using MasterCard with Lamar:");
        var card1 = lamarContainer.GetInstance<ICreditCard>("mastercard");
        var client1 = lamarContainer.GetInstance<Shopper>();
        client1.Checkout(card1);

        // Get VisaCard instance by name
        Console.WriteLine("\nUsing VisaCard with Lamar:");
        var card2 = lamarContainer.GetInstance<ICreditCard>("visa");
        var client2 = lamarContainer.GetInstance<Shopper>();
        client2.Checkout(card2);

        // Dispose the container when done
        lamarContainer.Dispose();
    }
}
/*
 * // Intialization for the concrete classes
        // var card1 = new MasterCard();
        // var card2 = new VisaCard();
        // Shopper client = new Shopper();
        // client.Checkout(card2);

        // 1. Create container instance
        //CustomContainer container = new CustomContainer();

        // 2. Registration for types
        //container.Register<ICreditCard, MasterCard>();
        //container.Register<Shopper, Shopper>();

        // 3. Resolution for types to return concrete types
        //var card1 = container.Resolve<ICreditCard>();
        //var client1 = container.Resolve<Shopper>();

        // client1.Checkout(card1);

        // // override for registration
        // // any resolution after this registration will return "MasterCard"
        // container.Register<ICreditCard, MasterCard>();
        // client1.Checkout(card1);

        ////if non-generic & Original method version not private
        //container.Register(typeof(ICreditCard), typeof(VisaCard));

        //// 3. resolution for types to return concrete types
        //var card2 = container.Resolve<ICreditCard>();
        //var client2 = container.Resolve<Shopper>();

        //client2.Checkout(card2);
 */