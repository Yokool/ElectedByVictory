using System.Collections.Generic;
using System.Linq;

public class NameProviderCollection : IUnitTestNameProvider
{

    private List<IUnitTestNameProvider> providers = new List<IUnitTestNameProvider>();

    public NameProviderCollection()
    {

    }


    public NameProviderCollection(params IUnitTestNameProvider[] providers)
    {
        SetProviders(providers);
    }
    
    public void AddProvider(IUnitTestNameProvider provider)
    {
        providers.Add(provider);
    }

    public void SetProviders(IUnitTestNameProvider[] providers)
    {
        this.providers = providers.ToList();
    }


    public string GetUnitTestName()
    {
        string collectionName = "";
        for(int i = 0; i < providers.Count; ++i)
        {
            collectionName += providers[i].GetUnitTestName();
        }
        return collectionName;
    }
}
