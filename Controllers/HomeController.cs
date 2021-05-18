using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ElectronicsStore2.Models;
using System.Data.Entity;
using System.Net;
using System.Data.Entity.Validation;

namespace ElectronicsStore2.Controllers
{
    public class HomeController : Controller
    {
        private ElectronicsEntities db = new ElectronicsEntities();
        public ActionResult Index(string searchString, string ProductCategory, string ParentItem)
        {

            //PRODUCT CATEGORIES 

            //load categories from DB
            List<string> productCategories = new List<string>();

            var productCatQuery = from c in db.products
                                  orderby c.item_category
                                  select c.item_category;

            //adding unique values to the list
            productCategories.AddRange(productCatQuery.Distinct());

            //pass ViewBag so that View can find it
            ViewBag.ProductCategory = new SelectList(productCategories);

            //PARENT ITEMS

            //load categories from DB
            List<string> parentItem = new List<string>();

            var parentItemQuery = from pi in db.products
                                  orderby pi.item_parent
                                  select pi.item_parent;

            //adding unique values to the list
            parentItem.AddRange(parentItemQuery.Distinct());

            //pass ViewBag so that View can find it
            ViewBag.ParentItem = new SelectList(parentItem);


            //load products from DB
            var items = from i in db.products
                        select i;

            // if the "product category"(selection box) isn't empty (item isn't selected)
            if (!string.IsNullOrEmpty(ProductCategory))
            {
                items = items.Where(i => i.item_category == ProductCategory);
            }
            
            // if the "parent item "(selection box) isn't empty (item isn't selected)
            if (!string.IsNullOrEmpty(ParentItem))
            {
                items = items.Where(i => i.item_parent == ParentItem);
            }
            
            // if the "search string"(textbox) isn't empty (there is text there)
            if (!string.IsNullOrEmpty(searchString))
            {
                items = items.Where(i => i.item_name.Contains(searchString));
            }

            return View(items);
        }

        public ActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddProduct([Bind(Include = "id,item_parent,item_brand,item_name,item_category,item_price,item_stock,item_img,item_shortdesc")] product item)
        {
            if (item.item_img == null)
            {
                item.item_img = "https://bitsofco.de/content/images/2018/12/broken-1.png";
            }

            if (ModelState.IsValid)
            {
                db.products.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(item);
        }

        public ActionResult View(int? id)
        {

            // if there is no ID - return "bad request" page
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // find the "item" from the product ID
            product item = db.products.Find(id);

            //if no product is found, return a "not found" page.
            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }


        public ActionResult Edit(int? id)
        {

            // if there is no ID - return "bad request" page
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // find the "item" from the product ID
            product item = db.products.Find(id);

            //if no product is found, return a "not found" page.
            if (item == null)
            {
                return HttpNotFound();
            }


            return View(item);
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,item_parent,item_brand,item_name,item_category,item_price,item_stock,item_img,item_shortdesc")]product item)
        {
            if (item.item_img == null)
            {
                item.item_img = "https://bitsofco.de/content/images/2018/12/broken-1.png";
            }

            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(item);
        }

        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            product item = db.products.Find(id);

            if (item == null)
            {
                return HttpNotFound();
            }

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            product item = db.products.Find(id);
            db.products.Remove(item);
            db.SaveChanges();
            return RedirectToAction("index");
        }

    }

}