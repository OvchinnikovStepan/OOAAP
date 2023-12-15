using Hwdtech;
using Hwdtech.Ioc;
using Moq;

namespace SpaceBattle.Lib;

public class MacroCommandTests
    {
        public MacroCommandTests()
        {
        new InitScopeBasedIoCImplementationCommand().Execute();
        IoC.Resolve<Hwdtech.ICommand>("Scopes.Current.Set", IoC.Resolve<object>("Scopes.New", IoC.Resolve<object>("Scopes.Root"))).Execute();

        IoC.Resolve<Hwdtech.ICommand>("IoC.Register","Command.MacroCommand.SomeMacroCommand",(object[] args)=>{
            return new string[]{"FirstSubCommand","SecondSubCommand"};}).Execute();

     
        }
        [Fact]
        public void MacroCommand_Command_Init_Correctly()
        {
            var firstSubCommand=new Mock<ICommand>();
            var secondSubCommand=new Mock<ICommand>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register","FirstSubCommand",(object[] args)=>{return firstSubCommand.Object;}).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register","SecondSubCommand",(object[] args)=>{return secondSubCommand.Object;}).Execute();
            
            var macroCommand=new MacroCommand("Command.MacroCommand.SomeMacroCommand");
            macroCommand.Execute();

            firstSubCommand.Verify(mc => mc.Execute(), Times.Once());
            secondSubCommand.Verify(cfc => cfc.Execute(), Times.Once());
        }
        [Fact]
        public void MacroCommand_Disability_To_Execute_SubCommand_Caueses_Exeption()
        {
            var firstSubCommand=new Mock<ICommand>();
            var secondSubCommand=new Mock<ICommand>();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register","FirstSubCommand",(object[] args)=>{return firstSubCommand.Object;}).Execute();
            IoC.Resolve<Hwdtech.ICommand>("IoC.Register","SecondSubCommand",(object[] args)=>{return secondSubCommand.Object;}).Execute();
            firstSubCommand.Setup(f=>f.Execute()).Throws(new Exception()).Verifiable();

            var macroCommand=new MacroCommand("Command.MacroCommand.SomeMacroCommand");

            Assert.Throws<Exception>(macroCommand.Execute);
        }
    }
