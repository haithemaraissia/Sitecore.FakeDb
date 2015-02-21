﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Sitecore.Data;
using Xunit;

namespace Sitecore.FakeDb.Serialization.Tests
{
    public class DsDbTemplateTest
    {
        [Fact]
        public void ShouldSetName()
        {
            DsDbTemplate template = new DsDbTemplate("/sitecore/templates/Sample/Sample Item");

            template.Should().NotBeNull();
            template.Name.Should().BeEquivalentTo("Sample Item");
        }

        [Fact]
        public void ShouldSetItemId()
        {
            DsDbTemplate template = new DsDbTemplate("/sitecore/templates/Sample/Sample Item");

            template.Should().NotBeNull();
            template.ID.ShouldBeEquivalentTo(ID.Parse("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}"));
        }

        [Fact]
        public void ShouldSetTemplateId()
        {
            DsDbTemplate template = new DsDbTemplate("/sitecore/templates/Sample/Sample Item");

            template.Should().NotBeNull();
            template.TemplateID.ShouldBeEquivalentTo(TemplateIDs.Template);
        }

        [Fact]
        public void ShouldLoadFields()
        {
            DsDbTemplate template = new DsDbTemplate("/sitecore/templates/Sample/Sample Item");

            using (new Db()
                {
                    template
                })
            {
                template.Should().NotBeNull();

                ID titleFieldId = ID.Parse("{75577384-3C97-45DA-A847-81B00500E250}");
                ID textFieldId = ID.Parse("{A60ACD61-A6DB-4182-8329-C957982CEC74}");
                ID sharedFieldId = ID.Parse("{8F0BDC2B-1A78-4C29-BF83-C1C318186AA6}");

                template.Fields[titleFieldId].Name.Should().BeEquivalentTo("Title");
                template.Fields[titleFieldId].Shared.ShouldBeEquivalentTo(false);

                template.Fields[textFieldId].Name.Should().BeEquivalentTo("Text");
                template.Fields[textFieldId].Shared.ShouldBeEquivalentTo(false);

                template.Fields[sharedFieldId].Name.Should().BeEquivalentTo("Some shared field");
                template.Fields[sharedFieldId].Shared.ShouldBeEquivalentTo(true);
            }
        }

        [Fact]
        public void ShouldBeAppliedToDeserializedItem()
        {
            DsDbTemplate template = new DsDbTemplate("/sitecore/templates/Sample/Sample Item");
            DsDbItem item = new DsDbItem("/sitecore/content/Home");
            using (Db db = new Db()
                {
                    template,
                    item
                })
            {
                item.TemplateID.ShouldBeEquivalentTo(template.ID);
                
                ID titleFieldId = ID.Parse("{75577384-3C97-45DA-A847-81B00500E250}");

                item.Fields[titleFieldId].Value.ShouldBeEquivalentTo("Sitecore");
                template.Fields[titleFieldId].Should().NotBeNull();
            }
        }

        [Fact]
        public void ShouldLookupById()
        {
            ID templateId = ID.Parse("{76036F5E-CBCE-46D1-AF0A-4143F9B557AA}");

            DsDbTemplate template = new DsDbTemplate(templateId);

            template.Should().NotBeNull();
            template.Name.Should().BeEquivalentTo("Sample Item");
            template.TemplateID.ShouldBeEquivalentTo(TemplateIDs.Template);
        }

        [Fact]
        public void ShouldLoadFieldFromShortenedPath()
        {
            DsDbTemplate template = new DsDbTemplate(
                "/sitecore/templates/Sample/much deeper path needed/for testing deserialization of/fields from shortened paths/correctly/Some deep template");

            using (new Db()
                {
                    template
                })
            {
                template.Should().NotBeNull();

                ID someDeepFieldId = ID.Parse("{90DE36F0-7239-497D-AB36-5587DF34F669}");
                template.Fields.Count(f => f.ID == someDeepFieldId).ShouldBeEquivalentTo(1);
                template.Fields[someDeepFieldId].Name.Should().BeEquivalentTo("Some deep field");
                template.Fields[someDeepFieldId].Shared.Should().BeFalse();
            }
        }
    }
}
