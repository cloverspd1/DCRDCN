import * as React from 'react';
import * as ReactDom from 'react-dom';
import { Version } from '@microsoft/sp-core-library';
import {
  BaseClientSideWebPart,
  IPropertyPaneConfiguration,
  PropertyPaneTextField
} from '@microsoft/sp-webpart-base';

import * as strings from 'SuperAdminListViewWebPartStrings';
import SuperAdminListView from './components/SuperAdminListView';
import { ISuperAdminListViewProps } from './components/ISuperAdminListViewProps';

import pnp from "sp-pnp-js";
export interface ISuperAdminListViewWebPartProps {
  description: string;
}

export default class SuperAdminListViewWebPart extends BaseClientSideWebPart<ISuperAdminListViewWebPartProps> {


  public onInit(): Promise<void> {
    
    return super.onInit().then(_ => {
  
      pnp.setup({
        spfxContext: this.context
      });
      
    });
  }

  public render(): void {
    const element: React.ReactElement<{} > = React.createElement(
      SuperAdminListView
     
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
