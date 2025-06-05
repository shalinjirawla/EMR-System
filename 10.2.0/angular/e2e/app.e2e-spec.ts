import { EMRSystemTemplatePage } from './app.po';

describe('EMRSystem App', function () {
    let page: EMRSystemTemplatePage;

    beforeEach(() => {
        page = new EMRSystemTemplatePage();
    });

    it('should display message saying app works', () => {
        page.navigateTo();
        expect(page.getParagraphText()).toEqual('app works!');
    });
});
