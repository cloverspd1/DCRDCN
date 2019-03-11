import * as React from 'react';
import * as ReactDom from 'react-dom';
import { Version } from '@microsoft/sp-core-library';
import {
  BaseClientSideWebPart,
  IPropertyPaneConfiguration,
  PropertyPaneTextField
} from '@microsoft/sp-webpart-base';

import * as strings from 'FbFormWebPartStrings';
import FbForm from './components/FbForm';
import { IFbFormProps } from './components/IFbFormProps';
import pnp from "sp-pnp-js";
export interface IFbFormWebPartProps {
  description: string;
  context:any;
}

export default class FbFormWebPart extends BaseClientSideWebPart<IFbFormWebPartProps> {
  public onInit(): Promise<void> {
    
      return super.onInit().then(_ => {
    
        pnp.setup({
          spfxContext: this.context
        });
        
      });
    }
   
  public render(): void {
   // console.log(this.context);
    const element: React.ReactElement<IFbFormProps > = React.createElement(
      FbForm,
      {
        description: this.properties.description,
        context: this.context
      }
    );

    ReactDom.render(element, this.domElement);
  }

  protected get dataVersion(): Version {
    return Version.parse('1.0');
  }

  protected getPropertyPaneConfiguration(): IPropertyPaneConfiguration {
    return {
      pages: [
        {
          header: {
            description: strings.PropertyPaneDescription,
      
          },
          groups: [
            {
              groupName: strings.BasicGroupName,
              groupFields: [
                PropertyPaneTextField('description', {
                  label: strings.DescriptionFieldLabel
                })
              ]
            }
          ]
        }
      ]
    };
  }
}
