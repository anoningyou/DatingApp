import { Component, OnInit } from '@angular/core';
import  ImageLinks from '../constants/image-links';
import { Message } from '../_models/messages';
import { Pagination } from '../_models/pagination';
import { MessageService } from '../_services/message.service';


@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages?: Message[];
  pagination?: Pagination;
  container = 'Unread';
  pageNumber = 1;
  pagesize = 5;
  loading = false;

  userLinks = ImageLinks.user;

  constructor(private messageService: MessageService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(){
    this.loading = true;
    this.messageService.getMessages(this.pageNumber, this.pagesize, this.container).subscribe({
      next: responce => {
        this.messages = responce.result;
        this.pagination = responce.pagination;
        this.loading = false
      }
    })
  }

  deleteMessage(id: number){
    this.messageService.deleteMessage(id).subscribe({
      next: () => this.messages?.splice(this.messages.findIndex(m=>m.id ===id),1)
    })
  }



  pageChanged(event: any){
    if(this.pageNumber !=event.page){
      this.pageNumber = event.page;
      this.loadMessages();
    }
  }

}
