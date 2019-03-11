import * as React from 'react';
import * as ReactDom from 'react-dom';
import { Version } from '@microsoft/sp-core-library';
import {
  BaseClientSideWebPart,
  IPropertyPaneConfiguration,
  PropertyPaneTextField
} from '@microsoft/sp-webpart-base';

import * as strings from 'SuperAdminFormWebPartStrings';
import SuperAdminForm from './components/SuperAdminForm';
import { ISuperAdminFormProps } from './components/ISuperAdminFormProps';
import pnp from "sp-pnp-js";

export interface ISuperAdminFormWebPartProps {
  description: string;
  context:any;
}

export default class SuperAdminFormWebPart extends BaseClientSideWebPart<ISuperAdminFormWebPartProps> {

  public onInit(): Promise<void> {
    
    return super.onInit().then(_ => {
  
      pnp.setup({
        spfxContext: this.context
      });
      
    });
  }

  public render(): void {
    const element: React.ReactElement<ISuperAdminFormProps > = React.createElement(
      SuperAdminForm,
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
            description: strings.PropertyPaneDescription
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
