import * as React from 'react';
import * as ReactDom from 'react-dom';
import { Version } from '@microsoft/sp-core-library';
import {
  BaseClientSideWebPart,
  IPropertyPaneConfiguration,
  PropertyPaneTextField
} from '@microsoft/sp-webpart-base';

import * as strings from 'MyRequestListViewWebPartStrings';
import MyRequestListView from './components/MyRequestListView';
import { IMyRequestListViewProps } from './components/IMyRequestListViewProps';
import pnp from "sp-pnp-js";
export interface IMyRequestListViewWebPartProps {
  description: string;
}

export default class MyRequestListViewWebPart extends BaseClientSideWebPart<IMyRequestListViewWebPartProps> {

  public onInit(): Promise<void> {
    
    return super.onInit().then(_ => {
  
      pnp.setup({
        spfxContext: this.context
      });
      
    });
  }
  public render(): void {
    const element: React.ReactElement<{} > = React.createElement(
      MyRequestListView
      
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
