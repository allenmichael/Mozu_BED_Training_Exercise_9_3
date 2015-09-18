using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mozu.Api;
using Autofac;
using Mozu.Api.ToolKit.Config;
using System.Collections.Generic;

namespace Mozu_BED_Training_Exercise_9_3
{
    [TestClass]
    public class MozuDataConnectorTests
    {
        private IApiContext _apiContext;
        private IContainer _container;

        [TestInitialize]
        public void Init()
        {
            _container = new Bootstrapper().Bootstrap().Container;
            var appSetting = _container.Resolve<IAppSetting>();
            var tenantId = int.Parse(appSetting.Settings["TenantId"].ToString());
            var siteId = int.Parse(appSetting.Settings["SiteId"].ToString());

            _apiContext = new ApiContext(tenantId, siteId);
        }

        [TestMethod]
        public void Exercise_9_1_Get_Attribute()
        {
            /* --Create a New Product Attribute Resource
             *     Resources are used to leverage the methods provided by the SDK to talk to the Mozu service
             *     via the Mozu REST API. Every resource takes an ApiContext object as a parameter.
             */
            var productAttributeResource = new Mozu.Api.Resources.Commerce.Catalog.Admin.Attributedefinition.AttributeResource(_apiContext);
            /*
             * --Utilize the Product Attribute Resource to Get All Product Attributes Returned as an AttributeCollection
             *     Product attributes are the properties of the product with differing types based on your needs.
             *     Define your product attribute as Options, Properties, or Extras
             *     —the following properties are accessible from a Product Attribute object:
             *     productAttribute.AdminName -- string
             *     productAttribute.AttributeCode -- string
             *     productAttribute.AttributeDataTypeSequence -- int
             *     productAttribute.AttributeFQN -- string
             *     productAttribute.AttributeMetadata -- List<AttributeMetadataItem>
             *     productAttribute.AttributeSequence -- int
             *     productAttribute.AuditInfo -- AuditInfo
             *     productAttribute.Conent -- Content (This object contains the name, description, and locale code for identifying language and country.)
             *     productAttribute.DataType -- string
             *     
             *     (Types include "List", "Text box", "Text area", "Yes/No", and "Date")
             *     productAttribute.InputType -- string
             *     
             *     (Used to identify the characteristics of an attribute)
             *     productAttribute.IsExtra -- bool
             *     productAttribute.IsOption -- bool
             *     productAttribute.IsProperty -- bool
             *     
             *     productAttribute.LocalizedContent -- List<AttributeLocalizedContent>
             *     productAttribute.MasterCatalogId -- int
             *     productAttribute.Namespace -- string 
             *     productAttribute.SearchSettings -- SearchSettings
             *     
             *     (Used to store properties such as max/min date, max/min numeric value, max/min string length, or a regular expression)
             *     productAttribute.Validation -- Validation
             *     productAttribute.ValueType -- string
             *     
             *     (Used to store predefined values, such as options in a List)
             *     productAttribute.VocabularyValues -- List<AttributeVocabularyValue>
             * 
             * 
             *     See the following sites for more info:
             *     http://developer.mozu.com/content/learn/appdev/product-admin/product-attribute-definition.htm
             *     http://developer.mozu.com/content/api/APIResources/commerce/commerce.catalog/commerce.catalog.admin.attributedefinition.attributes.htm
             */
            var productAttributes = productAttributeResource.GetAttributesAsync(startIndex: 0, pageSize: 200).Result;

            //Add Your Code: 
            //Write Total Count of Attributes

            System.Diagnostics.Debug.WriteLine(string.Format("Product Attributes Total Count: {0}", productAttributes.TotalCount));

            //Add Your Code: 
            //Get an Attribute by Fully Qualified Name

            var individualAttribute = productAttributeResource.GetAttributeAsync("tenant~rating").Result;

            //Note: AttributeFQN (fully qualified name) follows the naming convention tenant~attributeName

            //Add Your Code:
            //Write the Attribute Data Type

            System.Diagnostics.Debug.WriteLine(string.Format("Product Attribute Data Type[{0}]: {1}", individualAttribute.AttributeCode, individualAttribute.DataType));

            //Add Your Code:
            //Write the Attribute Input Type

            System.Diagnostics.Debug.WriteLine(string.Format("Product Attribute Input Type[{0}]: {1}", individualAttribute.AttributeCode, individualAttribute.InputType));

            //Add Your Code:
            //Write the Attribute Characteristics 

            System.Diagnostics.Debug.WriteLine(string.Format("Product Attribute Characteristic [{0}]: An Extra? {1}, An Option? {2}, A Property? {3}", individualAttribute.AttributeCode, individualAttribute.IsExtra, individualAttribute.IsOption, individualAttribute.IsProperty));

            //Or...

            WriteAttributeCharacteristic(individualAttribute);

            //Add Your Code: 
            if(individualAttribute.VocabularyValues != null)
            {
                foreach (var value in individualAttribute.VocabularyValues)
                {
                    //Write vocabulary values
                    System.Diagnostics.Debug.WriteLine(string.Format("Product Attribute Vocabulary Values[{0}]: Value({1}) StringContent({2})", individualAttribute.AttributeCode, value.Value, value.Content.StringValue));
                }
            }

            //Add Your Code: 
            //Get an Attribute filtered by name
            //Note: See this page for more info about filters:
            //http://developer.mozu.com/content/api/APIResources/StandardFeatures/FilteringAndSortingSyntax.htm

            var filteredAttributes = productAttributeResource.GetAttributesAsync(filter: "adminName sw 'a'").Result;

            var singleAttributeFromFiltered = new Mozu.Api.Contracts.ProductAdmin.Attribute();

            if(filteredAttributes.TotalCount > 0)
            {
                singleAttributeFromFiltered = filteredAttributes.Items[0];
            }

            //Add Your Code:
            if (singleAttributeFromFiltered.AttributeCode != null)
            {
                //Write the Attribute Data Type

                System.Diagnostics.Debug.WriteLine(string.Format("Product Attribute Data Type[{0}]: {1}", singleAttributeFromFiltered.AttributeCode, singleAttributeFromFiltered.DataType));

                //Write the Attribute Input Type

                System.Diagnostics.Debug.WriteLine(string.Format("Product Attribute Input Type[{0}]: {1}", singleAttributeFromFiltered.AttributeCode, singleAttributeFromFiltered.InputType));

                //Write the Attribute Characteristics

                System.Diagnostics.Debug.WriteLine(string.Format("Product Attribute Characteristic [{0}]: An Extra? {1}, An Option? {2}, A Property? {3}", singleAttributeFromFiltered.AttributeCode, singleAttributeFromFiltered.IsExtra, singleAttributeFromFiltered.IsOption, singleAttributeFromFiltered.IsProperty));

                //Or...

                WriteAttributeCharacteristic(singleAttributeFromFiltered);

                if (singleAttributeFromFiltered.VocabularyValues != null)
                {
                    foreach (var value in singleAttributeFromFiltered.VocabularyValues)
                    {
                        //Write vocabulary values
                        System.Diagnostics.Debug.WriteLine(string.Format("Product Attribute Vocabulary Values[{0}]: Value({1}) StringContent({2})", singleAttributeFromFiltered.AttributeCode, value.Value, value.Content.StringValue));
                    }
                }
            }
        }

        [TestMethod]
        public void Exercise_9_2_Add_New_Attribute_Extra()
        {
            //Once again, we'll be using a Product Attribute resource
            var productAttributeResource = new Mozu.Api.Resources.Commerce.Catalog.Admin.Attributedefinition.AttributeResource(_apiContext);

            /*
             * We use a contract to declare an empty Product Attribute that we will populate with our data and then send via the API.
             * 
             * Contracts define the data model that each object adheres to -- each contract has different required fields.
             * The API documentation is useful for viewing some required fields, but trial and error is needed in some cases as well.
             * 
             * We viewed the Product Attribute model in Exercise 9.1, but here I'll define out the required fields for creating a new Attribute.
             * 
             * newAttribute.AdminName -- string
             * newAttribute.AttributeCode -- string
             * newAttribute.DataType -- string (Types include "String", "ProductCode", and "Number"
             *                       --          Not all combinations of DataType and InputType are valid)  
             * newAttribute.InputType -- string (Types include "List", "TextBox", "TextArea", "Yes/No", and "Date"; 
             *                        --         Depending on the type you choose, other fields may be required)
             * 
             * (One of these Characteristics must be true)
             * newAttribute.isExtra -- bool
             * newAttribute.isOption -- bool
             * newAttribute.isProperty -- bool
             * 
             * newAttribute.ValueType -- string (Types include "Predefined", "AdminEntered", and "ShopperEntered"
             *                        --          "Predefined" cannot be used when creating a new attribute and exist only on attributes included with Mozu
             *                        --          Use "AdminEntered" or "ShopperEntered" to tell Mozu who sets this attribute.
             * 
             * You may encounter this error when creating Attributes:
             * "Attribute must have a valid configuration for an Attribute Type Rule.  
             * The combination of the attribute's values if it is an option, extra, or property 
             * for the selected data type, input type, and value type is invalid."
             * 
             * To solve for this error, continue to try changing the DataType, InputType, or Characteristics.
             * 
             */
            var newAttribute = new Mozu.Api.Contracts.ProductAdmin.Attribute()
            {
                //A name used solely within Mozu Admin
                AdminName = "monogram",

                //Used to uniquely identify an Attribute -- Mozu creates the AttributeFQN by adding "tenant~" to the AttributeCode
                //It's best practice to use lowercase for the AttributeCode
                AttributeCode = "monogram",

                //Defines the data entered by either Admin or Shopper users
                DataType = "String",

                //Defines the input form
                InputType = "TextBox",

                //Sets the Characteristic of the Attribute
                IsExtra = true,

                //Another contract -- AttributeLocalizedContent allows you to add a shopper-facing label
                Content = new Mozu.Api.Contracts.ProductAdmin.AttributeLocalizedContent()
                {
                    Name = "Monogram"
                },
                
                //Defines how the Attribute if an Admin or Shopper enters -- allows Shoppers to configure products with custom input.
                ValueType = "ShopperEntered"
            };

            //Add Your Code:
            //Create new attribute

            //Best Practice Tip:
            //Check if the attribute already exists before adding a new attribute.
            //The responseFields parameter allows you to choose which particular fields the Mozu API will return.
            var existingAttributes = productAttributeResource.GetAttributesAsync(filter: string.Format("AttributeCode sw '{0}'", newAttribute.AttributeCode), responseFields: "AttributeFQN").Result;

            //Verify that the attribute doesn't already exist by checking the TotalCount.
            if (existingAttributes.TotalCount == 0)
            {
                //Creating a new attribute returns an Attribute object if successful
                var resultingAttribute = productAttributeResource.AddAttributeAsync(newAttribute).Result;

                //Add Your Code:
                //Update the attribute search settings 
                //Create the settings in your local model
                resultingAttribute.SearchSettings = new Mozu.Api.Contracts.ProductAdmin.AttributeSearchSettings()
                {
                    SearchableInAdmin = true,
                    SearchableInStorefront = true,
                    SearchDisplayValue = true
                };

                //And update the attribute via the API -- the responseField allows you to only return the AttributeFQN 
                //For later use, requests like GetAttributeAsync require the AttributeFQN
                var resultingUpdatedAttribute = productAttributeResource.UpdateAttributeAsync(resultingAttribute, resultingAttribute.AttributeFQN, responseFields: "AttributeFQN").Result;
            }

            //Alternatively, you can use a try/catch block to handle errors returned from the Mozu API:
            try
            {
                var resultingAttribute = productAttributeResource.AddAttributeAsync(newAttribute).Result;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.InnerException.Message);
            }
        }

        [TestMethod]
        public void Exercise_9_3_Add_Attributes()
        {

            //Another usage of a ProductAttributeResource
            var productAttributeResource = new Mozu.Api.Resources.Commerce.Catalog.Admin.Attributedefinition.AttributeResource(_apiContext);

            //We create a new attribute using a Contract just like in the previous exercise
            var newAttribute = new Mozu.Api.Contracts.ProductAdmin.Attribute()
            {
                AdminName = "purse-size",
                AttributeCode = "purse-size",
                DataType = "String",
                InputType = "List",
                IsOption = true,
                Content = new Mozu.Api.Contracts.ProductAdmin.AttributeLocalizedContent()
                {
                    Name = "Purse Size"
                },
                ValueType = "Predefined",
                VocabularyValues = new List<Mozu.Api.Contracts.ProductAdmin.AttributeVocabularyValue>() 
                { 
                    //Since this is an Option attribute, we must add VocabularyValues.
                    //Here, we can add one Value at a time
                    new Mozu.Api.Contracts.ProductAdmin.AttributeVocabularyValue()
                    {
                        Value = "Petite",
                        Content = new Mozu.Api.Contracts.ProductAdmin.AttributeVocabularyValueLocalizedContent() 
                        {
                            LocaleCode = "en-US",
                            StringValue = "Petite"
                        }
                    }
                }
            };

            //Or, we can automate the process a bit with a collection of sizes that we then iterate over.
            var sizes = "Classic|Alta".Split('|');

            foreach(var size in sizes)
            {
                newAttribute.VocabularyValues.Add(new Mozu.Api.Contracts.ProductAdmin.AttributeVocabularyValue() 
                { 
                    Value = size,
                    Content = new Mozu.Api.Contracts.ProductAdmin.AttributeVocabularyValueLocalizedContent() 
                    {
                        LocaleCode = "en-US",
                        StringValue = size
                    }
                });
            }

            //Add Your Code:
            //Check if attribute already exists, return back only the attributeFQN 
            var existingAttributes = productAttributeResource.GetAttributesAsync(filter: string.Format("AttributeCode sw '{0}'", newAttribute.AttributeCode), responseFields: "AttributeFQN").Result;

            //Verify that the attribute doesn't exist
            if (existingAttributes.TotalCount == 0)
            {
                //Add Your Code: Add New Attribute
                var createdAttribute = productAttributeResource.AddAttributeAsync(newAttribute).Result;
            }

        }

        /// <summary>
        /// Helper method for checking the characteristic of a returned Product Attribute
        /// </summary>
        /// <param name="Attribute"></param>
        private void WriteAttributeCharacteristic(Mozu.Api.Contracts.ProductAdmin.Attribute individualAttribute)
        {
            System.Diagnostics.Debug.Write(string.Format("Product Attribute Characteristic[{0}]: ", individualAttribute.AttributeCode));

            if ((bool)individualAttribute.IsExtra)
            {
                System.Diagnostics.Debug.Write("Is an Extra");
            }
            else if ((bool)individualAttribute.IsOption)
            {
                System.Diagnostics.Debug.Write("Is an Option");
            }
            else if ((bool)individualAttribute.IsProperty)
            {
                System.Diagnostics.Debug.Write("Is a Property");
            }
            else
            {
                System.Diagnostics.Debug.Write("Has no characteristic");
            }
        }
    }
}
