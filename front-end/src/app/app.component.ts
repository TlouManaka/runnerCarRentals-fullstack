import { Component } from '@angular/core';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { HomeComponent } from './pages/home/home.component';
import {MatIconModule} from '@angular/material/icon';
import { HttpClient } from '@angular/common/http';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './components/navbar/navbar.component';




@Component({
  selector: 'app-root',
  imports: [MatSlideToggleModule,RouterOutlet,NavbarComponent],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'front-end';
}
