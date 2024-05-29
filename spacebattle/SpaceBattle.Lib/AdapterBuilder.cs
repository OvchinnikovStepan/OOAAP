namespace SpaceBattle.Lib;
using Scriban;

public class AdapterBulider : IBuilder
{
    private readonly Type inside_type;
    private readonly Type interface_type;

    public AdapterBulider(Type inside_type, Type interface_type)
    {
        this.inside_type = inside_type;
        this.interface_type = interface_type;
    }
    public string Build()
    {
        var interface_properties = interface_type.GetProperties().ToList();

        var templateString = @"public class {{interface_type_name}}Adapter : {{interface_type_name}} {
        {{inside_type_name}} _obj;
    
        public {{interface_type_name}}Adapter({{inside_type_name}} obj)
        {
         this.obj = obj;
        }
    {{for property in ( interface_properties_templating)}}
    public {{property.property_type.name}} {{property.name}}
    {
    {{if property.can_read}}
        get
        {
            return IoC.Resolve<{{property.property_type.name}}>(""Game.Get.Property"", ""{{property.name}}"", obj);
        }{{end}}
    {{if property.can_write}}
        set
        {
            return IoC.Resolve<ICommand>(""Game.Set.Property"", ""{{property.name}}"", obj, value).Execute();
        }{{end}}
    }
    {{end}}
    }";
        var template = Template.Parse(templateString);
        var templatedString = template.Render(new
        {
            interface_type_name = interface_type.Name,
            inside_type_name = inside_type.Name,
            interface_properties_templating = interface_properties,
        });
        return templatedString;
    }
}
