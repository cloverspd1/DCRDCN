import * as React from 'react';
import * as ReactDom from 'react-dom';
import { Version } from '@microsoft/sp-core-library';
import {
  BaseClientSideWebPart,
  IPropertyPaneConfiguration,
  PropertyPaneTextField
} from '@microsoft/sp-webpart-base';

import * as strings from 'HomePagePendingWebPartWebPartStrings';
import HomePagePendingWebPart from './components/HomePagePendingWebPart';
import { IHomePagePendingWebPartProps } from './components/IHomePagePendingWebPartProps';
import pnp from "sp-pnp-js";

export interface IHomePagePendingWebPartWebPartProps {
  description: string;
}

export default class HomePagePendingWebPartWebPart extends BaseClientSideWebPart<IHomePagePendingWebPartWebPartProps> {

  public onInit(): Promise<void> {
    
    return super.onInit().then(_ => {
  
      pnp.setup({
        spfxContext: this.context
      });
      
    });
  }


  public render(): void {
    const element: React.ReactElement<{}> = React.createElement(
      HomePagePendingWebPart
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
