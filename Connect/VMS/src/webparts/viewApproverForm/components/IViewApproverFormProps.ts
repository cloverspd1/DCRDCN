import { IWebPartContext } from "@microsoft/sp-webpart-base";

export interface IViewApproverFormProps {
  description: string;
  context:IWebPartContext;
}
export interface ISPList {
  ID: string;
  Title:string;
  Category:string;
  SubCategory :string;
  Description:string;
  Status: string;
  ApproverComment : string;
  CreatedDate :string;
  ApprovedBy :string;
  CreatedBy :string;
  ApprovedRejectedDate :string;
  Attachments :string;
  FileName :string;
  } 
