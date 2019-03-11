using BEL.CommonDataContract;
using BEL.DataAccessLayer;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace TestJob
{
    class Program
    {
        static void Main(string[] args)
        {
            string rootURL = "https://bajajelect.sharepoint.com/sites/ItemCodePreProDev/";
            using (ClientContext clientContext = BELDataAccessLayer.Instance.CreateClientContext(rootURL))
            {
                Web web = BELDataAccessLayer.Instance.CreateWeb(clientContext);
                List approverList = web.Lists.GetByTitle("ItemCodeApprovalMatrix");

                ListItemCollectionPosition position = null;
                var page = 1;
                do
                {
                    CamlQuery query = new CamlQuery();
                    DateTime? yesterDayDate = null;
                    int actionStatus = 1;
                    int id = 358 ;
                    query.ViewXml = @"<View>
                                               <Query>
                                                   <Where>
                                                      <Eq>
                                                         <FieldRef Name='RequestID' />
                                                         <Value Type='Lookup'>"+ id +@"</Value>
                                                      </Eq>
                                                   </Where>
                                                </Query>
                                                <RowLimit>5000</RowLimit>
                                            </View>";
                    query.ListItemCollectionPosition = position;

                    ListItemCollection approverDetails = approverList.GetItems(query);
                    clientContext.Load(approverDetails);
                    clientContext.ExecuteQuery();

                    position = approverDetails.ListItemCollectionPosition;

                    if (approverDetails != null && approverDetails.Count > 0)
                    {
                        for (int i = 0; i < approverDetails.Count; i++)
                        {
                         
                        }
                        page++;
                    }
                } while (position != null);
            }
        }
    }
}
