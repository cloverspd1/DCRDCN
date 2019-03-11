import { IWebPartContext } from "@microsoft/sp-webpart-base";

export interface IFbFormProps {
  description: string;
  context:IWebPartContext;
}

export interface ISPList {
ID: string;
Title:string;
Category:string;
SubCategory :string;
Description:string;
} 


export interface IKeyText {
  key: number;
  text:string;
} 
