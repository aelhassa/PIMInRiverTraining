using System;
using System.Globalization;
using inRiver.Remoting;
using inRiver.Remoting.Objects;
using inRiver.Remoting.Query;

class TestClass
{
    static RemoteManager _manager;
    static void Main(string[] args)
    {
        //Before u need to create the entity type in the control center 
        try
        {
            initializeRemoteManager();
            createProductEntity();
            Getproduct(1234);
            
            //Before u need to create the type in control center 
            CreateItem();
            LinkSKUToProduct(122,133);
            SearchByCriteria();
            SearchByLinkQuery();
            SearchByComplexeQuery();
        }catch(Exception ex ){
            Console.WriteLine(ex.ToString());
        }       
    }

    private static void SearchByComplexeQuery()
    {
        LinkQuery lq= new LinkQuery(){
            LinkTypeId="",
            Direction=LinkDirection.OutBound,//from product to sku
            SourceEntityTypeId="",
            TargetEntityTypeId="",
           
        };
        SystemQuery sq= new SystemQuery{
            LastModified=DateTime.Now.AddHours(-1),
            LastModifiedOperator=Operator.GreaterThan

        };
        ComplexQuery cq= new ComplexQuery{
            LinkQuery=lq,
            SystemQuery=sq
        };
        var data=_manager.DataService.Search(cq,LoadLevel.DataAndLinks);
        foreach(Entity product in data){
            Console.WriteLine(product.DisplayName.Data);
        }  
    }

    private static void SearchByLinkQuery()
    {
        LinkQuery lq= new LinkQuery(){
            LinkTypeId="",
            Direction=LinkDirection.OutBound,//from product to sku
            SourceEntityTypeId="",
            TargetEntityTypeId="",
            TargetCriteria=new List<Criteria>{
                new Criteria{
                    FieldTypeId="",
                    Operator=Operator.Contains,
                    Value=""
                }
            }
        };
        var allProductsWithLinkedSKU=_manager.DataService.LinkSearch(lq,LoadLevel.DataAndLinks);
        foreach(Entity product in allProductsWithLinkedSKU){
            Console.WriteLine(product.DisplayName.Data);
        }  
            
    }

    private static void SearchByCriteria()
    {
        Criteria productCriteria=new Criteria{
            FieldTypeId="",
            Operator=Operator.Contains,
            Value=""
        };
        List<Entity> data=_manager.DataService.Search(productCriteria,LoadLevel.DataOnly);
        foreach(Entity pro in data){
                Console.WriteLine(pro.DisplayName.Data);
        }
    }

    private static void LinkSKUToProduct(int productId, int  itemId)
    {
        Entity product=_manager.DataService.GetEntity(productId,LoadLevel.DataAndLinks);
        Entity sku=_manager.DataService.GetEntity(itemId,LoadLevel.DataAndLinks);
        
        List<LinkType> likeTypes= _manager.ModelService.GetLinkTypesForEntityType(product.EntityType.Id);
        LinkType productToSKULinkType=likeTypes.Find(x=>x.SourceEntityTypeId==product.EntityType.Id
                                                   && x.TargetEntityTypeId==sku.EntityType.Id);
        Link producttoitemLink=new Link{
            LinkType=productToSKULinkType,
            Source=product,
            Target=sku
        };
        _manager.DataService.AddLinkLast(producttoitemLink);
        Console.WriteLine($"Added link");
    }

    private static void CreateItem()
    {
        EntityType productEntityType = _manager.ModelService.GetEntityType("MyTrainingProduct");
        Entity SKUEntity=Entity.CreateEntity(productEntityType);
        Field SKUtId= SKUEntity.GetField("");
        Field SKUName= SKUEntity.GetField("");
        SKUtId.Data="blabla1";
        SKUName.Data="blabla133";

        var entityAdded=_manager.DataService.AddEntity(SKUEntity);
        Console.WriteLine($"Added SKU Entity Id {entityAdded.Id}");
        
    }

    private static void Getproduct(int entityId)
    {
        Entity entityToUpdate=_manager.DataService.GetEntity(entityId,LoadLevel.DataOnly);
        Field descField=entityToUpdate.GetField("");
        LocaleString descData= (LocaleString)descField.Data;
        descData[new CultureInfo("en")]="Test eng";
        descData[new CultureInfo("fr-fr")]="Test fran";
        descField.Data=descData;
        _manager.DataService.UpdateEntity(entityToUpdate);
    }

    private static void createProductEntity()
    {
        EntityType productEntityType = _manager.ModelService.GetEntityType("MyTrainingProduct");
        Entity productEntity=Entity.CreateEntity(productEntityType);
        Field productId= productEntity.GetField("");
        Field productName= productEntity.GetField("");
        productId.Data="blabla1";
        productName.Data="blabla133";

        var entityAdded=_manager.DataService.AddEntity(productEntity);
        Console.WriteLine($"Added Entity Id {entityAdded.Id}");
        

    }

    private static void initializeRemoteManager()
    {
        try
        {
         _manager = RemoteManager.CreateInstance("","");
          //user with API Key

        }catch(Exception ex){
            Console.WriteLine(ex);
        }
       
     
       

    }
}
