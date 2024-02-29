import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Product } from '../../models/product';
import { ProductService } from '../../services/product.service';

@Component({
  selector: 'app-edit-product',
  templateUrl: './edit-product.component.html',
  styleUrl: './edit-product.component.css'
})
export class EditProductComponent {
  @Input() product?: Product;
  @Output() productsUpdated = new EventEmitter<Product[]>();

  constructor(private productService: ProductService) {}

  updateProduct(product:Product)
  {
    this.productService.updateProduct(product).subscribe((products: Product[]) => this.productsUpdated.emit(products));
  }

  deleteProduct(product:Product)
  {
    this.productService.createProduct(product).subscribe((products: Product[]) => this.productsUpdated.emit(products));
  }

  createProduct(product:Product)
  {
    this.productService.deleteProduct(product).subscribe((products: Product[]) => this.productsUpdated.emit(products));
  }
}
