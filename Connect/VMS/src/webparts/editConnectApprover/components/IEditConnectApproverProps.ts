import { IWebPartContext } from "@microsoft/sp-webpart-base";

export interface IEditConnectApproverProps {
  description: string;
  context:IWebPartContext;
}

export interface ISPList {
  ID: string;
  Title:string;
  Category:string;
  SubCategory :string;
  Description:string;
  Status :string;
 // Approver :string;
  ApproverComment :string;
 // ConnectID:string;
  Created:string;
  Creator:string;
  ReasonForReject : string;
  Attachments :string;
  FileName :string;
  } 

  export interface IKeyText {
    key: number;
    text:string;
  } 


