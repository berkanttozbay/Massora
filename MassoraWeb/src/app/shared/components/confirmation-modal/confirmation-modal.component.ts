import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-confirmation-modal',
  templateUrl: './confirmation-modal.component.html',
  styleUrls: ['./confirmation-modal.component.css']
})
export class ConfirmationModalComponent {

  // Bu modal'ı çağıran component'ten bu değerleri alacağız.
  @Input() title: string = 'Confirmation';
  @Input() itemName: string = 'bu kaydı';
  @Input() prompt: string = 'Are you sure you want to delete the record named'; // Mesajı özelleştirmek için
  @Input() confirmButtonText: string = 'Yes , delete it'; // Buton metni
  @Input() cancelButtonText: string = 'Hayır, Vazgeç';


  constructor(public activeModal: NgbActiveModal) {}

  // "Evet" butonuna basıldığında, modal'ı 'true' sonucuyla kapatır.
  confirm() {
    this.activeModal.close(true);
  }

  // "Hayır" veya (X) butonuna basıldığında, modal'ı 'dismiss' ile kapatır.
  decline() {
    this.activeModal.dismiss('cancel');
  }
}