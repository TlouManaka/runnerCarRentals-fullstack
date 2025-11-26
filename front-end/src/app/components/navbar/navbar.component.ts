import { Component } from '@angular/core';
import { MatIconRegistry, MatIcon } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import {MatIconModule} from '@angular/material/icon';
import { MatSlideToggle } from "@angular/material/slide-toggle";
import { RouterLink,RouterLinkActive } from '@angular/router';
import { MatButtonModule, MatButton } from '@angular/material/button';

@Component({
  selector: 'app-navbar',
  imports: [MatIcon, RouterLink, RouterLinkActive, MatButton],
  templateUrl: './navbar.component.html',
  styleUrl: './navbar.component.css'
})
export class NavbarComponent {



  /**
   *
   */
  constructor(
    private maticonRegistry: MatIconRegistry,
    private domSanitizer: DomSanitizer
  ) {
    
    this.maticonRegistry.addSvgIcon('logo',this.domSanitizer.bypassSecurityTrustResourceUrl('assets/logo.svg'))
  }
}
